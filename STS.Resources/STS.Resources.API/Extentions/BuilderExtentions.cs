using STS.Resources.Application.Services;
using STS.Resources.Application.Interfaces;
using STS.Resources.Infrastructure.Repositories;
using STS.Resources.Infrastructure.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using STS.Resources.API.Interceptors;

namespace STS.Resources.API.Extentions;

public static class BuilderExtentions
{
    public static WebApplicationBuilder AddSTSResourcesApi(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ConfigureEndpointDefaults(listen => listen.Protocols = HttpProtocols.Http2);
        });
        builder.AddAuthentication();
        builder.Services.AddGrpc(options =>
        {
            // options.Interceptors.Add<ResourceGuardInterceptor>();
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
        builder.Services.AddScoped<ITeamRepository, TeamRepository>();
        builder.Services.AddScoped<IStadiumRepository, StadiumRepository>();
        builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        builder.Services.AddScoped<ILeagueService, LeagueService>();
        builder.Services.AddScoped<ITeamService, TeamService>();
        builder.Services.AddScoped<IStadiumService, StadiumService>();
        builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
        builder.Services.AddInfrastructure(
            builder.Configuration,
            builder.Environment.IsDevelopment());

        return builder;
    }

    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority =  builder.Configuration["AuthServer:Authority"];
                options.MetadataAddress = builder.Configuration["AuthServer:MetadataAddress"] 
                                          ?? throw new NullReferenceException("AuthServer:MetadataAddress is null");
                options.Audience  =  builder.Configuration["AuthServer:Audience"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidIssuer = builder.Configuration["AuthServer:Issuers"];
                options.TokenValidationParameters.ValidateAudience = false;
                
            });
        builder.Services.AddAuthorization();
        return builder;
    }
}
