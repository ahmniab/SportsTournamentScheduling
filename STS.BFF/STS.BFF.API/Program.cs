using STS.BFF.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddStsApi();

var app = builder.Build();
app.UseStsApi();

app.Run();
