using STS.Resources.API.Services;

namespace STS.Resources.API.Extentions;

public static class AppExtentions
{
    public static WebApplication UseSTSResourcesApi(this WebApplication app)
    {
        app.MapGrpcService<LeagueGrpcService>();
        app.MapGrpcService<TeamGrpcService>();
        return app;
    }
}