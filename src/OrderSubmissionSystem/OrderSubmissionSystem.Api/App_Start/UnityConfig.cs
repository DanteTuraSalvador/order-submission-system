using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Unity;
using Unity.WebApi;
using Unity.Injection; // ADD THIS LINE
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Services;
using OrderSubmissionSystem.Infrastructure.Processors;
using OrderSubmissionSystem.Infrastructure.Formatters;
using OrderSubmissionSystem.Infrastructure.Uploaders;
using OrderSubmissionSystem.Infrastructure.Configuration;
using OrderSubmissionSystem.Domain.Enums;
using OrderSubmissionSystem.Api.Security;

namespace OrderSubmissionSystem.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = BuildContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            InitializeApiKeyValidator(container);
        }

        public static IUnityContainer BuildContainer()
        {
            var container = new UnityContainer();

            // API Key Store Registration
            container.RegisterFactory<IApiKeyStore>((c, t, n) =>
            {
                var apiKeysSetting = SecureConfigurationHelper.GetRequiredSetting("ApiKeysFile");
                string filePath;

                if (Path.IsPathRooted(apiKeysSetting))
                {
                    filePath = apiKeysSetting;
                }
                else
                {
                    filePath = HostingEnvironment.MapPath(apiKeysSetting);
                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        throw new InvalidOperationException(
                            $"Unable to resolve API keys file path '{apiKeysSetting}'. Ensure the file exists or provide an absolute path.");
                    }
                }

                return new FileApiKeyStore(filePath);
            });

            // Formatters Registration
            container.RegisterType<IOrderFileFormatter, CsvOrderFormatter>(nameof(OrderFileFormat.Csv));
            container.RegisterType<IOrderFileFormatter, XmlOrderFormatter>(nameof(OrderFileFormat.Xml));
            container.RegisterType<IOrderFileFormatter, JsonOrderFormatter>(nameof(OrderFileFormat.Json));
            container.RegisterType<IOrderFileFormatter, ExcelOrderFormatter>(nameof(OrderFileFormat.Excel));

            // FTP Uploaders Registration
            container.RegisterType<IFtpUploader, LocalFtpUploader>(nameof(FtpUploaderType.Local));
            container.RegisterType<IFtpUploader, AzureFtpUploader>(nameof(FtpUploaderType.Azure));
            container.RegisterType<IFtpUploader, AwsFtpUploader>(nameof(FtpUploaderType.Aws));

            // Processors Registration
            container.RegisterType<IOrderProcessor, SqlOrderProcessor>(nameof(ProcessorType.Sql));
            container.RegisterType<IOrderProcessor, FtpOrderProcessor>(nameof(ProcessorType.Ftp));

            // Main Service Registration
            container.RegisterFactory<IOrderSubmissionService>((c, t, n) =>
            {
                var processorType = ProcessorConfiguration.GetProcessorType();
                IOrderProcessor processor;

                if (processorType == ProcessorType.Ftp)
                {
                    var fileFormat = ProcessorConfiguration.GetFileFormat();
                    var ftpUploaderType = ProcessorConfiguration.GetFtpUploaderType();
                    var ftpSettings = ProcessorConfiguration.GetFtpUploaderSettings(ftpUploaderType);

                    IOrderFileFormatter formatter;
                    switch (fileFormat)
                    {
                        case OrderFileFormat.Csv:
                            formatter = new CsvOrderFormatter();
                            break;
                        case OrderFileFormat.Xml:
                            formatter = new XmlOrderFormatter();
                            break;
                        case OrderFileFormat.Json:
                            formatter = new JsonOrderFormatter();
                            break;
                        case OrderFileFormat.Excel:
                            formatter = new ExcelOrderFormatter();
                            break;
                        default:
                            formatter = new CsvOrderFormatter();
                            break;
                    }

                    IFtpUploader uploader;
                    switch (ftpUploaderType)
                    {
                        case FtpUploaderType.Azure:
                            uploader = new AzureFtpUploader(ftpSettings);
                            break;
                        case FtpUploaderType.Aws:
                            uploader = new AwsFtpUploader(ftpSettings);
                            break;
                        case FtpUploaderType.Local:
                            uploader = new LocalFtpUploader(ftpSettings);
                            break;
                        default:
                            uploader = new LocalFtpUploader(ftpSettings);
                            break;
                    }

                    processor = new FtpOrderProcessor(formatter, uploader);
                }
                else
                {
                    processor = new SqlOrderProcessor();
                }

                return new OrderSubmissionService(processor);
            });

            return container;
        }

        private static void InitializeApiKeyValidator(IUnityContainer container)
        {
            var apiKeyStore = container.Resolve<IApiKeyStore>();
            ApiKeyValidator.Initialize(apiKeyStore);
        }
    }
}
