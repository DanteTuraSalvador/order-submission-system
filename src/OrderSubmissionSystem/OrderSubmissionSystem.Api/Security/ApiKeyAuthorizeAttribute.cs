using OrderSubmissionSystem.Api.Monitoring;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace OrderSubmissionSystem.Api.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ApiKeyAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private readonly ILogger _logger = Log.ForContext<ApiKeyAuthorizeAttribute>();
        private readonly string _headerName;

        public ApiKeyAuthorizeAttribute(string headerName = "X-API-KEY")
        {
            _headerName = headerName;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var apiKey = ExtractApiKey(actionContext);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Deny(actionContext, HttpStatusCode.Unauthorized, "API key is required", "missing");
                return;
            }

            try
            {
                if (!ApiKeyValidator.TryValidate(apiKey))
                {
                    Deny(actionContext, HttpStatusCode.Unauthorized, "API key is invalid", "invalid");
                    return;
                }

                AuthMetrics.ApiKeyValidation.WithLabels("success").Inc();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while validating API key");
                Deny(actionContext, HttpStatusCode.InternalServerError, "Authentication service unavailable", "error");
            }
        }

        private string ExtractApiKey(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.TryGetValues(_headerName, out var headerValues))
            {
                return headerValues.FirstOrDefault();
            }

            var queryParameter = actionContext.Request.GetQueryNameValuePairs()
                .FirstOrDefault(p => string.Equals(p.Key, "apiKey", StringComparison.OrdinalIgnoreCase));

            return queryParameter.Value;
        }

        private void Deny(HttpActionContext actionContext, HttpStatusCode statusCode, string message, string metricLabel)
        {
            AuthMetrics.ApiKeyValidation.WithLabels(metricLabel).Inc();
            _logger.Warning("API key validation failed: {Result}", metricLabel);
            actionContext.Response = actionContext.Request.CreateResponse(statusCode, new { message });
        }
    }
}
