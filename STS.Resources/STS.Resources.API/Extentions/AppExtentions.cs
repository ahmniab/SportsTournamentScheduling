using STS.Resources.Application.Services;

namespace STS.Resources.API.Extentions;
public static class AppExtentions
{
    public static WebApplication UseSTSResourcesApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        return app;
    }
}