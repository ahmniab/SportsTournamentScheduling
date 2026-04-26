using Microsoft.AspNetCore.Authentication.JwtBearer;
using STS.BFF.API.Grpc;

namespace STS.BFF.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddStsApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        });
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.AddAuthentication();
        builder.AddGrpcServices();

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
        return builder;
    }

    public static WebApplicationBuilder AddGrpcServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<LeagueService.LeagueServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:LeagueServiceBaseUrl"] 
                                      ??  throw new NullReferenceException("GRPC:LeagueServiceBaseUrl is null"));
        });
        builder.Services.AddGrpcClient<StadiumService.StadiumServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:StadiumServiceBaseUrl"]
                                      ??  throw new NullReferenceException("GRPC:StadiumServiceBaseUrl is null"));
        });
        builder.Services.AddGrpcClient<TeamService.TeamServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:TeamServiceBaseUrl"]
                                      ?? throw new NullReferenceException("GRPC:TeamServiceBaseUrl is null"));
        });
        builder.Services.AddGrpcClient<TimeSlotService.TimeSlotServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:TimeSlotServiceBaseUrl"]
                                      ?? throw new NullReferenceException("GRPC:TimeSlotServiceBaseUrl is null"));
        });
        return builder;
    }
    
}