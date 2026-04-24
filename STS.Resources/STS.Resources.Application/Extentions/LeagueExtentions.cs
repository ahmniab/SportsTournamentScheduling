using STS.Resources.Domain.Entities;
using STS.Resources.Application.Models.Responses;
using STS.Resources.Application.Features.League;
using STS.Resources.Application.Features;

namespace STS.Resources.Application.Extentions;

public static class LeagueExtentions
{
    
    public static LeagueResponse BuildLeagueResponse(this League league, LeagueIncludeOptions? options)
    {
        LeagueResponse response = new LeagueResponse
        {
            Id = league.Id,
            OwnerId = league.OwnerId,
            Name = league.Name,
            StartDate = league.StartDate,
            CreatedAt = league.CreatedAt,
            LogoUrl = league.LogoUrl
        };
        
        if (options != null)
        {
            response.Teams = league.Teams.BuildTeamsSummary(options.IncludeTeams);
            response.Stadiums = league.Stadiums.BuildStadiumsResponse(options.IncludeStadiums);
            response.TimeSlots = league.TimeSlots.BuildTimeSlotsResponse(options.IncludeTimeSlots);
        }
        
        return response;
    }

    public static IEnumerable<TeamSummaryResponse>? BuildTeamsSummary(this ICollection<Team> teams, IncludeOption option)
    {
        IEnumerable<TeamSummaryResponse>? response ;

        switch (option)
        {
            case IncludeOption.INCLUDE_ID:
                response = teams.Select(t => new TeamSummaryResponse
                {
                    Id = t.Id.ToString()
                });
                break;
            case IncludeOption.INCLUDE_ALL:
                response = teams.Select(t => new TeamSummaryResponse
                {
                    Id = t.Id.ToString(),
                    Name = t.Name,
                    LogoUrl = t.LogoUrl
                });
                break;
            default:
                response = null;
                break;
        }

        return response;
    }

    public static IEnumerable<StadiumSummaryResponse>? BuildStadiumsResponse(this ICollection<Stadium> stadiums,
        IncludeOption option)
    {
        IEnumerable<StadiumSummaryResponse>? response;

        switch (option)
        {
            case IncludeOption.INCLUDE_ID:
                response = stadiums.Select(s => new StadiumSummaryResponse
                {
                    Id = s.Id.ToString()
                });
                break;
            case IncludeOption.INCLUDE_ALL:
                response = stadiums.Select(s => new StadiumSummaryResponse
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    LogoUrl = s.Logo
                });
                break;
            default:
                response = null;
                break;
        }

        return response;
    }

    public static IEnumerable<TimeSlotSummaryResponse>? BuildTimeSlotsResponse(this ICollection<TimeSlot> timeSlots, IncludeOption option)
    {
        IEnumerable<TimeSlotSummaryResponse>? response;

        switch (option)
        {
            case IncludeOption.INCLUDE_ID:
                response = timeSlots.Select(ts => new TimeSlotSummaryResponse
                {
                    Id = ts.Id.ToString()
                });
                break;
            case IncludeOption.INCLUDE_ALL:
                response = timeSlots.Select(ts => new TimeSlotSummaryResponse
                {
                    Id = ts.Id.ToString(),
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime
                });
                break;
            default:
                response = null;
                break;
        }

        return response;
    }
}