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
        private readonly ILogger<RabbitMessageSender> logger;
        private readonly IConfiguration configuration;

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
            var factory = new ConnectionFactory() { HostName = configuration["rabbit:host"], UserName = configuration["rabbit:user"], Password = configuration["rabbit:password"] };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: configuration["rabbit:topic"],
                                        type: "topic");

                var routingKey = header;
                var message = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: configuration["rabbit:topic"],
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
                logger.LogInformation($" [x] Sent '{routingKey}':'{message}'");
            }
        }
    }
}
