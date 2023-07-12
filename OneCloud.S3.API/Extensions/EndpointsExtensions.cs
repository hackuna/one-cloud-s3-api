using OneCloud.S3.API.EndPoints;

namespace OneCloud.S3.API.Extensions;

public static class EndpointsExtensions
{
    public static IApplicationBuilder UseStorageEndpoints(this IApplicationBuilder builder)
    {
        var app = builder as WebApplication ?? throw new NullReferenceException();

        app.MapGroup("api").WithOpenApi()
            .UseStorageEndPoints()
            .UseUsersEndPoints()
            .UseObjectsEndpoints()
            .UseBucketsEndpoints();

        return app;
    }
}
