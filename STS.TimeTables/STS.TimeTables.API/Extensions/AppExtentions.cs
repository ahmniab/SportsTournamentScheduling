using STS.TimeTables.API.Services;
using STS.TimeTables.Infrastructure.Persistence;

namespace STS.TimeTables.API.Extensions;

public static class AppExtensions
{
    public static WebApplication UseTimeTablesApi(this WebApplication app)
    {
        // Run migrations on startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TimeTableDbContext>();
            dbContext.Database.Migrate();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGrpcService<TimeTablesGrpcService>().RequireAuthorization();
        return app;
    }
}