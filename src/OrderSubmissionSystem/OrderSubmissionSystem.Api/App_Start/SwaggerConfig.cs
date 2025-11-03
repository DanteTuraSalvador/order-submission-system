using System.Web.Http;
using WebActivatorEx;
using OrderSubmissionSystem.Api;
using Swashbuckle.Application;
using System.Web.Hosting; // Add this for VirtualPathUtility if needed

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace OrderSubmissionSystem.Api
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "OrderSubmissionSystem.Api");

                    // Add API Key configuration
                    c.ApiKey("X-API-Key")
                        .Description("API Key Authentication")
                        .Name("X-API-Key")
                        .In("header");

                    // Add XML comments (include this line)
                    c.IncludeXmlComments(GetXmlCommentsPath());
                })
                .EnableSwaggerUi(c =>
                {
                    // Enable API key support in Swagger UI
                    c.EnableApiKeySupport("X-API-Key", "header");
                });
        }

        // Add this method inside the SwaggerConfig class
        private static string GetXmlCommentsPath()
        {
            return $@"{System.AppDomain.CurrentDomain.BaseDirectory}\bin\OrderSubmissionSystem.Api.XML";
        }
    }
}