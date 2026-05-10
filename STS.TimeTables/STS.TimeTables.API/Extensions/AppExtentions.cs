using STS.TimeTables.API.Services;

namespace STS.TimeTables.API.Extensions;

public static class AppExtensions
{
    public static WebApplication UseTimeTablesApi(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGrpcService<TimeTablesGrpcService>().RequireAuthorization();
        return app;
    }
}