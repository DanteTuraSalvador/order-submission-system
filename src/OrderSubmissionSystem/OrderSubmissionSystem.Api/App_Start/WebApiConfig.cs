using OrderSubmissionSystem.Api.Filters;
using Swashbuckle.Application; // Add this using
using Swashbuckle.Swagger;
using System;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace OrderSubmissionSystem.Api
{
    /// <summary>
    /// Provides configuration for Web API routes, formatters, filters, exception handling, and Swagger documentation.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers Web API configuration and services.
        /// </summary>
        /// <param name="config">The HTTP configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            config.Services.Replace(typeof(IExceptionLogger), new GlobalExceptionLogger());
            config.Filters.Add(new ValidateModelStateAttribute());

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            jsonFormatter.SerializerSettings.NullValueHandling =
                Newtonsoft.Json.NullValueHandling.Ignore;
            jsonFormatter.SerializerSettings.DateFormatHandling =
                Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}