using Microsoft.AspNetCore.Builder;

namespace API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseApplicationPipeline(this WebApplication app)
        {
            // Authentication MÜTLƏQ Authorization-dan əvvəl
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
