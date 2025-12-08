using FTM.API.Middleware;

namespace FTM.API.Extensions
{
    public static class FTAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseFTAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FTAuthorizationMiddleware>();
        }
    }
}
