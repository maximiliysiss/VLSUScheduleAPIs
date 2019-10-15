using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public interface IServiceConnectionFactory
    {
        IServiceConnection Create(string name, List<string> models, ILogger logger, string host = "localhost");
    }

    public class RabbitMQServiceConnectionFactory : IServiceConnectionFactory
    {
        public IServiceConnection Create(string name, List<string> models, ILogger logger, string host = "localhost")
        {
            logger.LogInformation($"Create RabbitMQ Service Host:{host}");
            var service = new RabbitService(name, models, logger, host);
            service.Init();
            return service;
        }
    }
}
