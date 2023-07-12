using Microsoft.AspNetCore.Http.Json;

namespace OneCloud.S3.API.Extensions;

public static class JsonOptionsExtensions
{
    public static IServiceCollection AddJsonOptionsConfiguration(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(opt =>
        {
            opt.SerializerOptions.PropertyNamingPolicy = null;
            opt.SerializerOptions.DictionaryKeyPolicy = null;
        });

        return services;
    }
}
