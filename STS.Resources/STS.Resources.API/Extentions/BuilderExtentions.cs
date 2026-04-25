using STS.Resources.Application.Services;
using STS.Resources.Infrastructure.Extentions;
using STS.Resources.Application.Interfaces;
using STS.Resources.Infrastructure.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace STS.Resources.API.Extentions;

public static class BuilderExtentions
{
    public static WebApplicationBuilder AddSTSResourcesApi(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5166, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
        builder.Services.AddGrpc();
        builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
        builder.Services.AddScoped<ITeamRepository, TeamRepository>();
        builder.Services.AddScoped<IStadiumRepository, StadiumRepository>();
        builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        builder.Services.AddScoped<ILeagueService, LeagueService>();
        builder.Services.AddScoped<ITeamService, TeamService>();
        builder.Services.AddScoped<IStadiumService, StadiumService>();
        builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
        builder.Services.AddInfrastructure(
            builder.Configuration.GetConnectionString("ResourcesDb")!,
            builder.Configuration.GetConnectionString("Redis")!,
            builder.Configuration.GetSection("RabbitMq"),
            builder.Environment.IsDevelopment());

        return builder;
    }
}
