using BigBang1112.GbxTools.Titles.BlazorWebApp.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDomainServices();
builder.Services.AddDataServices(builder.Configuration, builder.Environment);
builder.Services.AddWebServices(builder.Configuration, builder.Environment);
builder.Services.AddTelemetryServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseMiddleware();

app.Run();
