using Microsoft.AspNetCore.Server.Kestrel.Core;
using STS.TimeTables.Application.Extensions;
using STS.TimeTables.Infrastructure.Extensions;

namespace STS.TimeTables.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddTimeTablesApi(this WebApplicationBuilder builder)
    {
        // gRPC requires HTTP/2. Force every Kestrel(web server) endpoint (incl. plain HTTP / h2c)
        // to use HTTP/2 so gRPC clients can connect without TLS during local testing.
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ConfigureEndpointDefaults(listen => listen.Protocols = HttpProtocols.Http2);
        });

        builder.Services.AddGrpc();
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());
        builder.AddAuthentication();
        return builder;
    }
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority =  builder.Configuration["AuthServer:Authority"];
                options.MetadataAddress = builder.Configuration["AuthServer:MetadataAddress"] 
                                          ?? throw new NullReferenceException("AuthServer:MetadataAddress is null");
                options.Audience  =  builder.Configuration["AuthServer:Audience"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidIssuer = builder.Configuration["AuthServer:Issuers"];
                options.TokenValidationParameters.ValidateAudience = false;
                
            });
        builder.Services.AddAuthorization();
        return builder;
    }
}