using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace VLSUScheduleAPIs.Services
{
    public interface IMessageSender
    {
        void Init();
        void SendMessage(string header, object obj);
    }

    public class RabbitMessageSender : IMessageSender
    {
        ILogger<RabbitMessageSender> logger;
        IConfiguration configuration;

        public RabbitMessageSender(ILogger<RabbitMessageSender> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public void Init()
        {

        }

        public void SendMessage(string header, object obj)
        {
            var factory = new ConnectionFactory() { HostName = configuration["rabbit:host"] };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: configuration["rabbit:topic"],
                                        type: "topic");

                var routingKey = header;
                var message = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "topic_logs",
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                logger.LogInformation($" [x] Sent '{routingKey}':'{message}'");
            }
        }
    }
}
