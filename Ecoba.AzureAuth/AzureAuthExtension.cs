using Microsoft.AspNetCore.Builder;

namespace Ecoba.AzureAuth
{
    public static class AzureAuthExtension
    {
        public static IApplicationBuilder UseAzureAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AzureAuthMiddleware>();
        }
    }
}