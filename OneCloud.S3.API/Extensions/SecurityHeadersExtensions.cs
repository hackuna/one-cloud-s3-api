using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace OneCloud.S3.API.Extensions;

public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add(new KeyValuePair<string, StringValues>(HeaderNames.XXSSProtection, "1; mode=block"));
            context.Response.Headers.Add(new KeyValuePair<string, StringValues>(HeaderNames.ContentSecurityPolicy, "default-src 'none'; script-src 'self' 'unsafe-inline'; connect-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'"));
            context.Response.Headers.Add(new KeyValuePair<string, StringValues>(HeaderNames.XFrameOptions, "DENY"));
            context.Response.Headers.Add(new KeyValuePair<string, StringValues>(HeaderNames.XContentTypeOptions, "nosniff"));
            context.Response.Headers.Add(new KeyValuePair<string, StringValues>(HeaderNames.StrictTransportSecurity, "max-age=31536000;includeSubDomains;preload"));
            context.Response.Headers.Add(new KeyValuePair<string, StringValues>(HeaderNames.Server, AppDomain.CurrentDomain.FriendlyName));
            await next();
        });

        return app;
    }
}
