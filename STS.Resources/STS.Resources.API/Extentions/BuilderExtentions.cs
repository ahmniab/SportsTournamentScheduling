using STS.Resources.Application.Services;
using STS.Resources.Infrastructure.Extentions;
using STS.Resources.Application.Interfaces;
using STS.Resources.Infrastructure.Repositories;

namespace STS.Resources.API.Extentions;

public static class BuilderExtentions
{
    public static WebApplicationBuilder AddSTSResourcesApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();
        builder.Services.AddScoped<ILeagueService, LeagueService>();
        builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("ResourcesDb"));
        return builder;
    }
}
