using OpenCnpj.Core.Configurations;
using System.Threading.RateLimiting;

namespace OpenCnpj.WebApi.DependencyInjection;

public static class RateLimitersInjection
{
    public static IServiceCollection AddRateLimitersInjection(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = configuration.GetSection("RateLimitOptions").Get<RateLimiterConfigurations>()!;

        services.AddRateLimiter(options =>
        {
            options.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new ValueTask();
            };

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var userIp = httpContext.Connection.RemoteIpAddress!.ToString();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        userIp, _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitOptions.PermitLimit,
                            Window = TimeSpan.FromSeconds(rateLimitOptions.WindowInSeconds)
                        });
                }));
        });

        return services;
    }
}
