using Serilog;
using Serilog.Events;
using System;
using System.Configuration;
using System.IO;
using System.Web.Http;
namespace OrderSubmissionSystem.Api
{
    /// <summary>
    /// Represents the ASP.NET Web API application for the Order Submission System.
    /// Handles application-level events such as startup and shutdown.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Handles application startup logic for the Order Submission System API.
        /// Configures logging, registers Web API routes, and sets up dependency injection.
        /// </summary>
        protected void Application_Start()
        {
            ConfigureLogging();
            Log.Information("Starting OrderSubmissionSystem API");

            GlobalConfiguration.Configure(WebApiConfig.Register);
            UnityConfig.RegisterComponents();
        }

        /// <summary>
        /// Handles application shutdown logic for the Order Submission System API.
        /// Ensures that logging is properly closed and resources are released.
        /// </summary>
        protected void Application_End()
        {
            Log.Information("Stopping OrderSubmissionSystem API");
            Log.CloseAndFlush();
        }

        private static void ConfigureLogging()
        {
            var seqUrl = Environment.GetEnvironmentVariable("SeqUrl") ?? ConfigurationManager.AppSettings["SeqUrl"];
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var logDirectory = Path.Combine(baseDirectory, "App_Data", "logs");
            Directory.CreateDirectory(logDirectory);
            var logFilePath = Path.Combine(logDirectory, "ordersubmission-.log");

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, shared: true);

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Seq(seqUrl);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}



