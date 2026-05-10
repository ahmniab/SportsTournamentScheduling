import logging
import os

from handler import handle_message
from rabbitmq import RabbitMQClient
from redis_client import RedisClient

logger = logging.getLogger(__name__)

PREPARED_QUEUE = "matches.generator"
COMPLETED_QUEUE = "matches.completed"


def configure_scheduler():
    redis_client = RedisClient(
        host=os.getenv("REDIS_HOST", "localhost"),
        port=os.getenv("REDIS_PORT", 6379),
        user=os.getenv("REDIS_USER") or None,
        password=os.getenv("REDIS_PASSWORD") or None,
    )

    rabbitmq_client = RabbitMQClient(
        host=os.getenv("RABBITMQ_HOST", "localhost"),
        port=os.getenv("RABBITMQ_PORT", 5672),
        username=os.getenv("RABBITMQ_USER", "guest"),
        password=os.getenv("RABBITMQ_PASSWORD", "guest"),
    )

    if not redis_client.ping():
        raise ConnectionError("Cannot connect to Redis — check REDIS_* environment variables")

    rabbitmq_client.connect()

    def on_message(channel, method, properties, body):
        handle_message(
            redis_client=redis_client,
            rabbitmq_client=rabbitmq_client,
            completed_queue=COMPLETED_QUEUE,
            body=body,
            delivery_tag=method.delivery_tag,
            channel=channel,
        )

    logger.info("Service configured — listening on queue '%s'", PREPARED_QUEUE)
    rabbitmq_client.consume(PREPARED_QUEUE, on_message)
