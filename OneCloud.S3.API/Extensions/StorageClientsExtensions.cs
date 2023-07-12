using Amazon.S3;
using Microsoft.Net.Http.Headers;

namespace OneCloud.S3.API.Extensions;

public static class StorageClientsExtensions
{
    public static IServiceCollection AddStorageClient(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrEmpty(configuration["SERVICE_API_URL"]);
        ArgumentException.ThrowIfNullOrEmpty(configuration["SERVICE_API_KEY"]);
        ArgumentException.ThrowIfNullOrEmpty(configuration["S3_SERVICE_URL"]);
        ArgumentException.ThrowIfNullOrEmpty(configuration["S3_ACCESS_KEY"]);
        ArgumentException.ThrowIfNullOrEmpty(configuration["S3_SECRET_KEY"]);

        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri(configuration["SERVICE_API_URL"]!);
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, System.Net.Mime.MediaTypeNames.Application.Json);
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {configuration["SERVICE_API_KEY"]}");
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, AppDomain.CurrentDomain.FriendlyName);
        });

        services.AddScoped(_ => new AmazonS3Client(
            configuration["S3_ACCESS_KEY"],
            configuration["S3_SECRET_KEY"],
            new AmazonS3Config
            {
                ServiceURL = configuration["S3_SERVICE_URL"],
                ForcePathStyle = true
            }));

        return services;
    }
}
