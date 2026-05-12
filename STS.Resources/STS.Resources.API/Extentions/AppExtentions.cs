using STS.Resources.API.Services;
using STS.Resources.Infrastructure.Persistence;

namespace STS.Resources.API.Extentions;

public static class AppExtentions
{
    public static WebApplication UseSTSResourcesApi(this WebApplication app)
    {
        // Run migrations on startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ResourcesDbContext>();
            dbContext.Database.Migrate();
        }

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