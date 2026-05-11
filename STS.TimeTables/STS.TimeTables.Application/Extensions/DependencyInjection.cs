using Microsoft.Extensions.DependencyInjection;
using STS.TimeTables.Application.Features.LeagueCommands;
using STS.TimeTables.Application.Features.MatchCommands;

namespace STS.TimeTables.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetLeagueSummaryCommandHandler>();
        services.AddScoped<GetFullLeagueCommandHandler>();
        services.AddScoped<DeleteLeagueCommandHandler>();
        services.AddScoped<GetMatchCommandHandler>();
        services.AddScoped<GenerateTimeTableHandler>();
        services.AddScoped<SaveTimeTableHandler>();
        services.AddScoped<GetLeagueJobStatusCommandHandler>();

        return services;
    }
}
