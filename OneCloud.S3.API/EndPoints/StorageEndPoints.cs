using OneCloud.S3.API.Models;

namespace OneCloud.S3.API.EndPoints;

public static class StorageEndPoints
{
    public static IEndpointRouteBuilder UseStorageEndPoints(this RouteGroupBuilder builder)
    {
        var storage = builder.MapGroup("storage").WithTags("Storage");

        storage.MapPost("", async (IHttpClientFactory httpClientFactory, CancellationToken cancellationToken) =>
            {
                using var client = httpClientFactory.CreateClient("api");
                using var request = await client.PostAsync("storage", null, cancellationToken);
                return await request.Content.ReadFromJsonAsync<StorageApiDto>(cancellationToken: cancellationToken)
                    is { } response
                    ? TypedResults.Ok(response)
                    : Results.BadRequest();
            })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status403Forbidden)
            .WithName("StorageActivate")
            .WithSummary("Activate storage");

        storage.MapDelete("", async (IHttpClientFactory httpClientFactory, CancellationToken cancellationToken) =>
            {
                using var client = httpClientFactory.CreateClient("api");
                using var request = await client.DeleteAsync("storage", cancellationToken);
                return await request.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken)
                    is { } response
                    ? TypedResults.Ok(response)
                    : Results.BadRequest();
            })
            .Produces(StatusCodes.Status403Forbidden)
            .WithName("StorageDeactivate")
            .WithSummary("Deactivate storage");

        return builder;
    }
}
