using Serilog;
using Serilog.Events;
using System;
using System.Configuration;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OrderSubmissionSystem.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ConfigureLogging();
            Log.Information("Starting OrderSubmissionSystem API");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterComponents();
        }

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

