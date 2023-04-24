using OneCloud.S3.API.Infrastructure;
using OneCloud.S3.API.Infrastructure.Interfaces;
using OneCloud.S3.API.Models.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions"));
builder.Services.AddScoped<IStorageBucketRepository, StorageRepository>();
builder.Services.AddScoped<IStorageObjectRepository, StorageRepository>();
builder.Services.AddScoped<IStorageRepository, StorageRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
