using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControllerCommon.Services
{
    public class ConsulePositionCollection
    {
        public string Adress { get; set; }
        public string[] Tags { get; set; }
        public string this[string tag]
        {
            get
            {
                if (Tags.Contains(tag))
                    return tag;
                throw new Exception();
            }
        }
    }

    public interface IConsulWrapper
    {
        ConsulePositionCollection this[string config] { get; }
        IConsulClient ConsulClient { get; }
    }

    public class ConsulWrapper : IConsulWrapper
    {
        public IConsulClient consulClient;
        private readonly Dictionary<string, ConsulePositionCollection> configsModels = new Dictionary<string, ConsulePositionCollection>();

        public ConsulWrapper(IConsulClient consulClient)
        {
            this.consulClient = consulClient;
        }

        public ConsulePositionCollection this[string config]
        {
            get
            {
                if (configsModels.Keys.Contains(config))
                    return configsModels[config];
                configsModels.Clear();
                var services = consulClient.Agent.Services().GetAwaiter().GetResult();
                foreach (var serv in services.Response)
                {
                    var newServ = new ConsulePositionCollection
                    {
                        Adress = serv.Value.Address,
                        Tags = serv.Value.Tags,
                    };
                    configsModels.Add(serv.Value.Service, newServ);
                }
                return configsModels[config];
            }
        }

        public IConsulClient ConsulClient => consulClient;
    }
}
