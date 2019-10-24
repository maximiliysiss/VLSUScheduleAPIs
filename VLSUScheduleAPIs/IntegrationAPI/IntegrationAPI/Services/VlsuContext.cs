using Commonlibrary.Models;
using Consul;
using NetServiceConnection.NetContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationAPI.Services
{
    public class VlsuContext : NetContext
    {
        public VlsuContext(IConsulClient consulClient, IServiceProvider serviceProvider) : base(consulClient, serviceProvider)
        {
        }

        public NetSet<Auditory> Auditories { get; set; }
    }
}
