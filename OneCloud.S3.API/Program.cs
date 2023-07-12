using OneCloud.S3.API.Extensions;
using System.Net.Mime;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.CaptureStartupErrors(true);

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddResponseCompression();
builder.Services.AddJsonOptionsConfiguration();
builder.Services.AddStorageClient(builder.Configuration);

if(builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseResponseCompression();
app.UseSecurityHeaders();

if(app.Environment.IsDevelopment() || app.Environment.IsStaging())
{

    app.UseExceptionHandler();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

if(app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapGet("/", () =>
        Results.Redirect("/swagger/", false, false))
        .ExcludeFromDescription();
}
else
{
    app.MapGet("/", () =>
        Results.Content("Service is healthy", MediaTypeNames.Text.Plain, Encoding.UTF8, StatusCodes.Status200OK))
        .ExcludeFromDescription();
}

app.UseStorageEndpoints();

app.Run();
