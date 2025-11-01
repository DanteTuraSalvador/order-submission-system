using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OrderSubmissionSystem.Api.Filters
{
    ///
    /// Custom authentication filter for API Key validation
    /// TODO: Implement Redis validation in next phase
    ///
    public class ApiKeyAuthAttribute : AuthorizationFilterAttribute
    {
        private const string API_KEY_HEADER = "X-API-Key";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Skip auth for health check endpoint
            if (actionContext.Request.RequestUri.AbsolutePath.Contains("/health"))
            {
                return;
            }

            var headers = actionContext.Request.Headers;

            if (!headers.Contains(API_KEY_HEADER))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { message = "API Key is missing" });
                return;
            }

            var apiKey = headers.GetValues(API_KEY_HEADER).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { message = "API Key is invalid" });
                return;
            }

            // TODO: Validate API Key against Redis
            // For now, accept any non-empty key
            // We'll implement Redis validation in Issue #46
        }
    }
}