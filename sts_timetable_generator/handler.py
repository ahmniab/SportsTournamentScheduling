import json
import logging
import math
from datetime import datetime, timedelta, timezone

from core_scheduler import run_tournament_scheduler

logger = logging.getLogger(__name__)

SCHEDULED_KEY_PREFIX = "league:scheduled:"


def _calculate_days(num_teams, num_venues, num_slots, start_date_str):
    """
    Generate calendar day labels starting from start_date.

    Round-robin: n*(n-1)/2 total matches.
    Per day at most: floor(n/2) matches (team uniqueness) AND
                     num_venues * num_slots slots (capacity).
    We add a 50% buffer so the GA has room to spread games out.
    """
    total_matches = num_teams * (num_teams - 1) // 2
    matches_per_day = min(num_teams // 2, num_venues * num_slots)

    if matches_per_day == 0:
        raise ValueError("Not enough venues/slots to schedule any matches per day")

    min_days = math.ceil(total_matches / matches_per_day)
    buffered_days = math.ceil(min_days * 1.5)
    num_days = max(buffered_days, min_days + 2)

    try:
        start_date = datetime.fromisoformat(
            start_date_str.replace("Z", "+00:00")
        ).replace(tzinfo=timezone.utc)
    except (ValueError, AttributeError):
        start_date = datetime.now(tz=timezone.utc)
        logger.warning("Could not parse StartDate '%s', using today", start_date_str)

    return [
        (start_date + timedelta(days=i)).strftime("%Y-%m-%d")
        for i in range(num_days)
    ]


def build_scheduler_config(league_data):
    """Transform Redis league payload into core_scheduler config dict."""
    teams = [t["Id"] for t in league_data["Teams"]]
    venues = [s["Id"] for s in league_data["Stadiums"]]
    daily_slots = [ts["Id"] for ts in league_data["TimeSlots"]]

    days = _calculate_days(
        num_teams=len(teams),
        num_venues=len(venues),
        num_slots=len(daily_slots),
        start_date_str=league_data.get("StartDate", ""),
    )

    logger.info(
        "Built scheduler config: %d teams, %d venues, %d slots/day, %d days",
        len(teams),
        len(venues),
        len(daily_slots),
        len(days),
    )

    return {
        "teams": teams,
        "venues": venues,
        "daily_slots": daily_slots,
        "days": days,
    }


def format_schedule_result(league_data, scheduler_result):
    """
    Convert raw scheduler output (list of 5-tuples) into a serializable dict.
    Each match tuple: (team1_id, team2_id, day, time_slot_id, venue_id)
    """
    matches = [
        {
            "Team1Id": team1,
            "Team2Id": team2,
            "Date": day,
            "TimeSlotId": time_slot,
            "StadiumId": venue,
        }
        for team1, team2, day, time_slot, venue in scheduler_result["schedule"]
    ]

    return {
        "Id": league_data["Id"],
        "Matches": matches,
        "BestFitness": scheduler_result["best_fitness"],
        "MatchesCount": scheduler_result["matches_count"],
        "TeamsCount": scheduler_result["teams_count"],
        "VenuesCount": scheduler_result["venues_count"],
        "GeneratedAt": datetime.now(tz=timezone.utc).isoformat(),
    }


def handle_message(redis_client, rabbitmq_client, completed_queue, body, delivery_tag, channel):
    """
    Process a single incoming RabbitMQ message.
    Expected body: {"RedisKey": "league:prepared:<uuid>"}
    """
    try:
        payload = json.loads(body)
    except (json.JSONDecodeError, TypeError) as e:
        logger.error("Failed to parse message body: %s — body: %r", e, body)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return

    redis_key = payload.get("RedisKey")
    league_id = payload.get("LeagueId")
    if not redis_key:
        logger.error("Message missing 'RedisKey' field: %r", payload)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return

    logger.info("Received scheduling request for key: %s", redis_key)

    league_data = redis_client.get_smart(redis_key)
    if league_data is None:
        logger.error("No data found in Redis for key: %s", redis_key)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return
    
    league_job_data = redis_client.get_smart(f"jobs:generate_league:{league_id}")
    if league_job_data is None:
        logger.error("No data found in Redis for key: %s", redis_key)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return
    league_job_data["Status"] = 2  # Mark as Generating
    redis_client.set_json(f"jobs:generate_league:{league_id}", league_job_data)

    redis_client.delete(redis_key)
    logger.info("Deleted prepared key from Redis: %s", redis_key)

    league_id = league_data.get("Id")
    if not league_id:
        logger.error("League data missing 'Id' field for key: %s", redis_key)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return

    try:
        config = build_scheduler_config(league_data)
        logger.info("Running scheduler for league %s...", league_id)
        result = run_tournament_scheduler(config)
        logger.info(
            "Scheduler finished for league %s — fitness=%.2f, matches=%d",
            league_id,
            result["best_fitness"],
            result["matches_count"],
        )
    except Exception as e:
        logger.exception("Scheduler failed for league %s: %s", league_id, e)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return

    scheduled_key = f"{SCHEDULED_KEY_PREFIX}{league_id}"
    formatted = format_schedule_result(league_data, result)

    try:
        redis_client.set_json(scheduled_key, formatted)
        logger.info("Saved schedule to Redis key: %s", scheduled_key)
    except Exception as e:
        logger.exception("Failed to save schedule to Redis for league %s: %s", league_id, e)
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return

    completion_event = json.dumps({
        "RedisKey": scheduled_key,
        "LeagueId": league_id,
    })

    try:
        rabbitmq_client.publish(completed_queue, completion_event)
        logger.info("Published completion event for league %s to '%s'", league_id, completed_queue)
    except Exception as e:
        logger.exception(
            "Failed to publish completion event for league %s: %s", league_id, e
        )
        channel.basic_nack(delivery_tag=delivery_tag, requeue=False)
        return

    channel.basic_ack(delivery_tag=delivery_tag)
    logger.info("Message acknowledged for league %s", league_id)
