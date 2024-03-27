using Microsoft.AspNetCore.Http.Features;
using OneCloud.S3.API.Infrastructure;
using OneCloud.S3.API.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.CaptureStartupErrors(true);

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = long.MaxValue);
builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = long.MaxValue);

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services
    .AddScoped<IStorageBucketRepository, StorageRepository>()
    .AddScoped<IStorageObjectRepository, StorageRepository>();

builder.Services.AddControllers();

if(builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapControllers();

app.Run();
