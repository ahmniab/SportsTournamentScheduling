using Google.Protobuf.WellKnownTypes;
using STS.Resources.API.Grpc;
using STS.Resources.Application.Features.League;

namespace STS.Resources.API.Extentions;

using AppLeagueResponse = Application.Models.Responses.LeagueResponse;
using AppLeagueIncludeOptions = Application.Features.League.LeagueIncludeOptions;
using GrpcLeagueResponse = LeagueResponse;
using AppIncludeOption = Application.Features.IncludeOption;

public static class LeagueGrpcMappingExtentions
{
    public static GetLeagueByIdCommand ToGetLeagueByIdCommand(this GetLeagueRequest request)
    {
        return new GetLeagueByIdCommand
        {
            Id = request.Id,
            IncludeOptions = request.Include is null
                ? null
                : new AppLeagueIncludeOptions
                {
                    IncludeTeams = request.Include.Teams.ToAppIncludeOption(),
                    IncludeStadiums = request.Include.Stadiums.ToAppIncludeOption(),
                    IncludeTimeSlots = request.Include.TimeSlots.ToAppIncludeOption()
                }
        };
    }

    public static GrpcLeagueResponse ToGrpcResponse(this AppLeagueResponse response, AppLeagueIncludeOptions? options)
    {
        var grpcResponse = new GrpcLeagueResponse
        {
            Id = response.Id.ToString(),
            OwnerId = response.OwnerId.ToString(),
            Name = response.Name,
            CreatedAt = response.CreatedAt.ToUtcTimestamp(),
            StartDate = response.StartDate.ToUtcTimestamp()
        };

        if (!string.IsNullOrWhiteSpace(response.LogoUrl))
        {
            grpcResponse.LogoUrl = response.LogoUrl;
        }

        if (options is not null)
        {
            if(options.IncludeTeams != AppIncludeOption.INCLUDE_NOTHING)
                grpcResponse.Teams.AddRange(BuildTeamSummaries(response.Teams, options.IncludeTeams));
            if(options.IncludeStadiums != AppIncludeOption.INCLUDE_NOTHING)
                grpcResponse.Stadiums.AddRange(BuildStadiumSummaries(response.Stadiums, options.IncludeStadiums));
            if (options.IncludeTimeSlots != AppIncludeOption.INCLUDE_NOTHING)
                grpcResponse.TimeSlots.AddRange(BuildTimeSlotSummaries(response.TimeSlots, options.IncludeTimeSlots));
        }

        return grpcResponse;
    }

    private static IEnumerable<TeamSummary> BuildTeamSummaries(
        IEnumerable<Application.Models.Responses.TeamSummaryResponse>? teams,
        AppIncludeOption option)
    {
        if (teams is null)
        {
            return [];
        }

        return option switch
        {
            AppIncludeOption.INCLUDE_ID => teams.Select(team => new TeamSummary
            {
                Id = team.Id
            }),
            AppIncludeOption.INCLUDE_ALL => teams.Select(team =>
            {
                var summary = new TeamSummary
                {
                    Id = team.Id
                };

                if (!string.IsNullOrWhiteSpace(team.Name))
                {
                    summary.Name = team.Name;
                }

                if (!string.IsNullOrWhiteSpace(team.LogoUrl))
                {
                    summary.LogoUrl = team.LogoUrl;
                }

                return summary;
            }),
            _ => []
        };
    }

    private static IEnumerable<StadiumSummary> BuildStadiumSummaries(
        IEnumerable<Application.Models.Responses.StadiumSummaryResponse>? stadiums,
        AppIncludeOption option)
    {
        if (stadiums is null)
        {
            return [];
        }

        return option switch
        {
            AppIncludeOption.INCLUDE_ID => stadiums.Select(stadium => new StadiumSummary
            {
                Id = stadium.Id
            }),
            AppIncludeOption.INCLUDE_ALL => stadiums.Select(stadium =>
            {
                var summary = new StadiumSummary
                {
                    Id = stadium.Id
                };

                if (!string.IsNullOrWhiteSpace(stadium.Name))
                {
                    summary.Name = stadium.Name;
                }

                if (!string.IsNullOrWhiteSpace(stadium.LogoUrl))
                {
                    summary.Logo = stadium.LogoUrl;
                }

                return summary;
            }),
            _ => []
        };
    }

    private static IEnumerable<TimeSlotSummary> BuildTimeSlotSummaries(
        IEnumerable<Application.Models.Responses.TimeSlotSummaryResponse>? timeSlots,
        AppIncludeOption option)
    {
        if (timeSlots is null)
        {
            return [];
        }

        return option switch
        {
            AppIncludeOption.INCLUDE_ID => timeSlots.Select(timeSlot => new TimeSlotSummary
            {
                Id = timeSlot.Id
            }),
            AppIncludeOption.INCLUDE_ALL => timeSlots.Select(timeSlot =>
            {
                var summary = new TimeSlotSummary
                {
                    Id = timeSlot.Id
                };

                if (!string.IsNullOrWhiteSpace(timeSlot.StartTime))
                {
                    summary.StartTime = timeSlot.StartTime;
                }

                if (!string.IsNullOrWhiteSpace(timeSlot.EndTime))
                {
                    summary.EndTime = timeSlot.EndTime;
                }

                return summary;
            }),
            _ => []
        };
    }

    private static AppIncludeOption ToAppIncludeOption(this API.Grpc.IncludeOption option)
    {
        return option switch
        {
            API.Grpc.IncludeOption.IncludeId => AppIncludeOption.INCLUDE_ID,
            API.Grpc.IncludeOption.IncludeAll => AppIncludeOption.INCLUDE_ALL,
            _ => AppIncludeOption.INCLUDE_NOTHING
        };
    }

    private static Timestamp ToUtcTimestamp(this DateTime value)
    {
        var utcValue = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return utcValue.ToTimestamp();
    }
}