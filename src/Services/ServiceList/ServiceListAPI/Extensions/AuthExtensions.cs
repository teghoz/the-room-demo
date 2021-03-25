using Microsoft.AspNetCore.Builder;

namespace ServiceListAPI.Extensions
{
    public static class AuthenticationServices
    {
        public static IApplicationBuilder ConfigureAuth(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
} 