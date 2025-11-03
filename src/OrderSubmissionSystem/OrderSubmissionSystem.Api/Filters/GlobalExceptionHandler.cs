
using Serilog;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace OrderSubmissionSystem.Api.Filters
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private static readonly ILogger _logger = Serilog.Log.ForContext<GlobalExceptionHandler>();

        public override void Handle(ExceptionHandlerContext context)
        {
            var correlationId = Guid.NewGuid().ToString();

            _logger.Error(
                context.Exception,
                "Unhandled exception in API request. Path: {Path}, Method: {Method}, CorrelationId: {CorrelationId}",
                context.Request.RequestUri?.AbsolutePath,
                context.Request.Method,
                correlationId);

            var content = new
            {
                error = "Internal Server Error",
                message = "An unexpected error occurred. Please contact support with the correlation ID.",
                correlationId,
                timestamp = DateTime.UtcNow
            };

            context.Result = new ResponseMessageResult(
                context.Request.CreateResponse(HttpStatusCode.InternalServerError, content));
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }
    public class GlobalExceptionLogger : ExceptionLogger
    {
        private static readonly ILogger _logger = Serilog.Log.ForContext<GlobalExceptionLogger>();

        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Error(
                context.Exception,
                "Exception logged. Path: {Path}, Method: {Method}, ExceptionType: {ExceptionType}",
                context.Request?.RequestUri?.AbsolutePath ?? "unknown",
                context.Request?.Method?.ToString() ?? "unknown",
                context.Exception?.GetType().Name ?? "unknown");
        }

        public override bool ShouldLog(ExceptionLoggerContext context)
        {
            return true;
        }
    }

    public class ValidateModelStateAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private static readonly ILogger _logger = Serilog.Log.ForContext<ValidateModelStateAttribute>();

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = new System.Collections.Generic.List<string>();
                foreach (var state in actionContext.ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.Add(error.ErrorMessage ?? error.Exception?.Message ?? "Unknown error");
                    }
                }

                _logger.Warning(
                    "Model validation failed. Path: {Path}, Errors: {Errors}",
                    actionContext.Request.RequestUri?.AbsolutePath,
                    string.Join("; ", errors));

                var response = new
                {
                    error = "Validation Failed",
                    message = "The request contains invalid data",
                    errors,
                    timestamp = DateTime.UtcNow
                };

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    response);
            }
        }
    }
}