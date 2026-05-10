using STS.Resources.API.Services;

namespace STS.Resources.API.Extentions;

public static class AppExtentions
{
    public static WebApplication UseSTSResourcesApi(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapGrpcService<LeagueGrpcService>()
            .RequireAuthorization();
        app.MapGrpcService<TeamGrpcService>()
            .RequireAuthorization();
        app.MapGrpcService<StadiumGrpcService>()
            .RequireAuthorization();
        app.MapGrpcService<TimeSlotGrpcService>()
            .RequireAuthorization();
        
        return app;
    }
}