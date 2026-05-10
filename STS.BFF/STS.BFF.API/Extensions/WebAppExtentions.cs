namespace STS.BFF.API.Extensions;

public static class WebAppExtensions
{
    public static WebApplication UseStsApi(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();           
        app.UseCors("AllowAll");    
        app.UseAuthorization();
        app.UseAuthentication();
        app.UseAuthorization();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(); 
            app.UseSwaggerUI(); 
        }
        app.MapControllers();
        
        return app;
    }

    
}