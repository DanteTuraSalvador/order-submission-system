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

            container.RegisterType<IOrderSubmissionService, OrderSubmissionService>(new HierarchicalLifetimeManager());

            var processorType = ProcessorConfiguration.GetProcessorType();

            if (processorType == ProcessorType.Sql)
            {
                container.RegisterType<IOrderProcessor, SqlOrderProcessor>(new HierarchicalLifetimeManager());
            }
            else
            {
                container.RegisterType<IOrderProcessor, FtpOrderProcessor>(new HierarchicalLifetimeManager());
            }

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}