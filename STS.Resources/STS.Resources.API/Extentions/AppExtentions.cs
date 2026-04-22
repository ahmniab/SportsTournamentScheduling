using STS.Resources.API.Services;

namespace STS.Resources.API.Extentions;

public static class AppExtentions
{
    public static WebApplication UseSTSResourcesApi(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.MapGrpcService<LeagueGrpcService>();
        return app;
    }
}