using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using STS.BFF.API.Features.League.Commands;
using STS.BFF.API.Grpc;
using STS.TimeTables.API.Grpc;

namespace STS.BFF.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddStsApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<GetFullTimeTableCommandHandler>();
        builder.AddAuthentication();
        builder.AddGrpcServices();

        return builder;
    }

    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "BFF-Session";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = builder.Configuration["AuthServer:Url"];
                options.MetadataAddress = builder.Configuration["AuthServer:MetadataAddress"];
                options.ClientId = "bff-client";
                options.ClientSecret = builder.Configuration["AuthServer:ClientSecret"];
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add("offline_access");
                options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });
        return builder;
    }

    public static WebApplicationBuilder AddGrpcServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<LeagueService.LeagueServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:LeagueServiceBaseUrl"]
                                      ?? throw new NullReferenceException("GRPC:LeagueServiceBaseUrl is null"));
        });
        builder.Services.AddGrpcClient<StadiumService.StadiumServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:StadiumServiceBaseUrl"]
                                      ?? throw new NullReferenceException("GRPC:StadiumServiceBaseUrl is null"));
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
        builder.Services.AddGrpcClient<TimeTablesService.TimeTablesServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GRPC:TimeTablesServiceBaseUrl"]
                                      ?? throw new NullReferenceException("GRPC:TimeTablesServiceBaseUrl is null"));
        });
        return builder;
    }

}