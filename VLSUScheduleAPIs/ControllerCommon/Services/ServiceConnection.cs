using ControllerCommon.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public interface IServiceConnectionFactory
    {
        IServiceConnection Create(string name, List<string> models, string host = "localhost");
        IServiceConnection Create(string name, string models, string host = "localhost");
    }

    public class RabbitMQServiceConnectionFactory : IServiceConnectionFactory
    {
        ILogger logger;
        private IConsulWrapper consulWrapper;

        public RabbitMQServiceConnectionFactory(ILogger logger, IConsulWrapper consulWrapper)
        {
            this.logger = logger;
            this.consulWrapper = consulWrapper;
        }

        public IServiceConnection Create(string name, List<string> models, string host = "localhost")
        {
            logger.LogInformation($"Create RabbitMQ Service Host:{host}");
            var service = new RabbitService(name, models, logger, host);
            service.Init();
            return service;
        }

        public IServiceConnection Create(string name, string models, string host = "localhost") => Create(name, new List<string> { models }, host);
    }
}
