using OrderSubmissionSystem.Domain.Enums;
using System;
using System.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Configuration
{
    public static class ProcessorConfiguration
    {
        public static ProcessorType GetProcessorType()
        {
            var processorTypeSetting = ConfigurationManager.AppSettings["ProcessorType"];

            if (!string.IsNullOrWhiteSpace(processorTypeSetting) &&
                 Enum.TryParse(processorTypeSetting, true, out ProcessorType processorType))
            {
                return processorType;
            }

            return ProcessorType.Sql;
        }
    }
}