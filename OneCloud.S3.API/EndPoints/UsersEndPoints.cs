using OneCloud.S3.API.Models;

namespace OneCloud.S3.API.EndPoints;

public static class UsersEndPoints
{
    public static IEndpointRouteBuilder UseUsersEndPoints(this IEndpointRouteBuilder builder)
    {
        var users = builder
            .MapGroup("storage/users")
            .WithTags("Users");

        users.MapGet("", async (IHttpClientFactory httpClientFactory, CancellationToken cancellationToken) =>
        {
            using var client = httpClientFactory.CreateClient("api");
            return await client.GetFromJsonAsync<StorageApiDto[]>("storage/users", cancellationToken)
                is { } response
                    ? TypedResults.Ok(response)
                    : Results.NotFound();
        })
            .Produces<StorageApiDto[]>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetUsers")
            .WithSummary("List of storage users");

        users.MapGet("{id:int}", async (IHttpClientFactory httpClientFactory, int id, CancellationToken cancellationToken) =>
        {
            using var client = httpClientFactory.CreateClient("api");
            return await client.GetFromJsonAsync<StorageApiDto>($"storage/users/{id}", cancellationToken)
                is { } response
                ? TypedResults.Ok(response)
                : Results.NotFound();
        })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetUser")
            .WithSummary("Storage user by Id");

        users.MapPost("", async (IHttpClientFactory httpClientFactory, string userName, bool persistPassword, CancellationToken cancellationToken) =>
        {
            using var client = httpClientFactory.CreateClient("api");
            using var request = await client.PostAsJsonAsync("storage/users",
                new { UserName = userName, PersistPassword = persistPassword }, cancellationToken);
            return await request.Content.ReadFromJsonAsync<StorageApiDto>(cancellationToken: cancellationToken)
                is { } response
                    ? TypedResults.Ok(response)
                    : Results.NotFound();
        })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("CreateUser")
            .WithSummary("Create new user");

        users.MapPost("{id:int}/block", async (IHttpClientFactory httpClientFactory, int id, CancellationToken cancellationToken) =>
        {
            using var client = httpClientFactory.CreateClient("api");
            using var request = await client.PostAsync($"storage/users/{id}/block", null, cancellationToken);
            return await request.Content.ReadFromJsonAsync<StorageApiDto>(cancellationToken: cancellationToken)
                is { } response
                ? TypedResults.Ok(response)
                : Results.NotFound();
        })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("BlockUser")
            .WithSummary("Block storage user");

        users.MapPost("{id:int}/unblock", async (IHttpClientFactory httpClientFactory, int id, CancellationToken cancellationToken) =>
        {
            using var client = httpClientFactory.CreateClient("api");
            using var request = await client.PostAsync($"storage/users/{id}/unblock", null, cancellationToken);
            return await request.Content.ReadFromJsonAsync<StorageApiDto>(cancellationToken: cancellationToken)
                is { } response
                ? TypedResults.Ok(response)
                : Results.NotFound();
        })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UnblockUser")
            .WithSummary("Unblock storage user");

        users.MapDelete("{id:int}", async (IHttpClientFactory httpClientFactory, int id, CancellationToken cancellationToken) =>
        {
            using var client = httpClientFactory.CreateClient("api");
            using var request = await client.DeleteAsync($"storage/users/{id}", cancellationToken);
            return await request.Content.ReadFromJsonAsync<StorageApiDto>(cancellationToken: cancellationToken)
                is { } response
                ? TypedResults.Ok(response)
                : Results.NotFound();
        })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteUser")
            .WithSummary("Delete storage user");

        users.MapPost("{id:int}/change-password",
                async (IHttpClientFactory httpClientFactory, int id, bool persistPassword, CancellationToken cancellationToken) =>
                {
                    using var client = httpClientFactory.CreateClient("api");
                    using var request = await client.PostAsync($"storage/users/{id}/change-password",
                        JsonContent.Create(new { PersistPassword = persistPassword }), cancellationToken);
                    return await request.Content.ReadFromJsonAsync<StorageApiDto>(cancellationToken: cancellationToken)
                        is { } response
                        ? TypedResults.Ok(response)
                        : Results.NotFound();
                })
            .Produces<StorageApiDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("UserChangePassword")
            .WithSummary("Reset password of storage user");

        return builder;
    }
}
