using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Services;
using OrderSubmissionSystem.Domain.Enums;
using OrderSubmissionSystem.Infrastructure.Configuration;
using OrderSubmissionSystem.Infrastructure.Processors;
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

            // Register Application Services
            container.RegisterType<IOrderSubmissionService, OrderSubmissionService>(new HierarchicalLifetimeManager());

            // Register Infrastructure - Processor based on configuration
            var processorType = ProcessorConfiguration.GetProcessorType();

            if (processorType == ProcessorType.Sql)
            {
                container.RegisterType<IOrderProcessor, SqlOrderProcessor>(new HierarchicalLifetimeManager());
            }
            else
            {
                container.RegisterType<IOrderProcessor, FtpOrderProcessor>(new HierarchicalLifetimeManager());
            }

            // Set Unity as the dependency resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}