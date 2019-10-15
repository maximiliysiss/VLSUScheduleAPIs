using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public interface IServiceConnection
    {
        void Init();
        void SendEventToChannel<T>(T ev);
    }

    public class RabbitService : IServiceConnection, IDisposable
    {
        private readonly string topic;
        private readonly string host;
        private readonly List<string> routingKeys;
        private readonly ILogger logger;
        private IConnection connection;
        private IModel channel;

        public RabbitService(string topic, List<string> routing, ILogger logger, string host = "localhost")
        {
            this.topic = topic;
            this.host = host;
            this.routingKeys = routing;
            this.logger = logger;
        }

        public void Dispose()
        {
            logger.LogInformation($"Disposing RabbitMQ Service Host:{host}, topic:{topic}");
            connection.Dispose();
            channel.Dispose();
        }

        public void Init()
        {
            logger.LogInformation($"Init RabbitMQ Service Host:{host}, topic:{topic}");
            var factory = new ConnectionFactory() { HostName = host };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: topic,
                                    type: "topic");
        }

        public void SendEventToChannel<T>(T ev)
        {
            var message = JsonConvert.SerializeObject(ev);
            var body = Encoding.UTF8.GetBytes(message);
            foreach (var route in routingKeys)
                channel.BasicPublish(exchange: topic,
                                 routingKey: route,
                                 basicProperties: null,
                                 body: body);
        }

        public void RegisterConsumer<T>(Action<T> action)
        {
            var queueName = channel.QueueDeclare().QueueName;
            foreach (var bindingKey in routingKeys)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: topic,
                                  routingKey: bindingKey);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body);
                logger.LogInformation($"Receive RabbitMQ Service Host:{host}, topic:{topic}, message:{body}");
                action(JsonConvert.DeserializeObject<T>(body));
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
