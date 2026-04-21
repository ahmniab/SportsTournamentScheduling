using STS.Resources.API.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSTSResourcesApi();
var app = builder.Build();

app.UseSTSResourcesApi();

app.Run();


