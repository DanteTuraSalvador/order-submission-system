using OrderSubmissionSystem.Api.Security;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Services;
using OrderSubmissionSystem.Domain.Enums;
using OrderSubmissionSystem.Infrastructure.Configuration;
using OrderSubmissionSystem.Infrastructure.Formatters;
using OrderSubmissionSystem.Infrastructure.Processors;
using OrderSubmissionSystem.Infrastructure.Uploaders;
using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Unity;
using Unity.Lifetime;

namespace OrderSubmissionSystem.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            RegisterApiKeyStore(container);
            container.RegisterType<IOrderSubmissionService, OrderSubmissionService>(new HierarchicalLifetimeManager());

            var processorType = ProcessorConfiguration.GetProcessorType();

            if (processorType == ProcessorType.Sql)
            {
                container.RegisterType<IOrderProcessor, SqlOrderProcessor>(new HierarchicalLifetimeManager());
            }
            else
            {
                RegisterFileFormatter(container);
                RegisterFtpUploader(container);
                container.RegisterType<IOrderProcessor, FtpOrderProcessor>(new HierarchicalLifetimeManager());
            }

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static void RegisterApiKeyStore(IUnityContainer container)
        {
            var fileSetting = ConfigurationManager.AppSettings["ApiKeysFile"] ?? "~/App_Data/api-keys.json";
            var mappedPath = HostingEnvironment.MapPath(fileSetting);
            string filePath;
            if (!string.IsNullOrEmpty(mappedPath))
            {
                filePath = mappedPath;
            }
            else if (Path.IsPathRooted(fileSetting))
            {
                filePath = fileSetting;
            }
            else
            {
                filePath = Path.GetFullPath(fileSetting);
            }

            container.RegisterFactory<IApiKeyStore>(_ => new FileApiKeyStore(filePath), new ContainerControlledLifetimeManager());
            ApiKeyValidator.Initialize(container.Resolve<IApiKeyStore>());
        }

        private static void RegisterFileFormatter(IUnityContainer container)
        {
            var format = ProcessorConfiguration.GetFileFormat();

            switch (format)
            {
                case OrderFileFormat.Csv:
                    container.RegisterType<IOrderFileFormatter, CsvOrderFormatter>(new HierarchicalLifetimeManager());
                    break;
                case OrderFileFormat.Json:
                    container.RegisterType<IOrderFileFormatter, JsonOrderFormatter>(new HierarchicalLifetimeManager());
                    break;
                case OrderFileFormat.Excel:
                    container.RegisterType<IOrderFileFormatter, ExcelOrderFormatter>(new HierarchicalLifetimeManager());
                    break;
                case OrderFileFormat.Xml:
                default:
                    container.RegisterType<IOrderFileFormatter, XmlOrderFormatter>(new HierarchicalLifetimeManager());
                    break;
            }
        }

        private static void RegisterFtpUploader(IUnityContainer container)
        {
            var uploaderType = ProcessorConfiguration.GetFtpUploaderType();
            container.RegisterFactory<FtpUploaderSettings>(_ => ProcessorConfiguration.GetFtpUploaderSettings(uploaderType), new HierarchicalLifetimeManager());

            switch (uploaderType)
            {
                case FtpUploaderType.Azure:
                    container.RegisterType<IFtpUploader, AzureFtpUploader>(new HierarchicalLifetimeManager());
                    break;
                case FtpUploaderType.Aws:
                    container.RegisterType<IFtpUploader, AwsFtpUploader>(new HierarchicalLifetimeManager());
                    break;
                case FtpUploaderType.Local:
                default:
                    container.RegisterType<IFtpUploader, LocalFtpUploader>(new HierarchicalLifetimeManager());
                    break;
            }
        }
    }
}
