using Commonlibrary.Models;
using CommonLibrary.Models;
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
        public VlsuContext(IConsulClient consulClient, IServiceProvider serviceProvider, Action<NetContext> configAction) : base(consulClient, serviceProvider, configAction)
        {
        }

        public NetSet<Auditory> Auditories { get; set; }
        public NetSet<Group> Groups { get; set; }
        public NetSet<IllCard> IllCards { get; set; }
        public NetSet<Lesson> Lessons { get; set; }
        public NetSet<Schedule> Schedules { get; set; }
        public NetSet<Institute> Institutes { get; set; }
        public NetSet<Teacher> Teachers { get; set; }
        public NetSet<Student> Students { get; set; }
    }
}
