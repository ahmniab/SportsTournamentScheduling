namespace STS.BFF.API.Extensions;

public static class WebAppExtensions
{
    public static WebApplication UseStsApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(); 
            app.UseSwaggerUI(); 
        }
        app.MapControllers();
        
        return app;
    }

    
}