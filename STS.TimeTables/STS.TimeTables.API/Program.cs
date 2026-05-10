using STS.TimeTables.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddTimeTablesApi();
var app = builder.Build();

app.UseTimeTablesApi();

app.Run();

