using Commonlibrary.Models;
using Consul;
using Microsoft.Extensions.Logging;
using NetServiceConnection.NetContext;
using System;

namespace VLSUScheduleAPIs.Services
{
    public class AuthNetContext : NetContext
    {
        public AuthNetContext(IConsulClient consulClient, IServiceProvider serviceProvider, Action<NetContext> configAction) : base(consulClient, serviceProvider, configAction)
        {
        }

        public NetSet<User> Users { get; set; }
        public NetSet<Teacher> Teachers { get; set; }
        public NetSet<Student> Students { get; set; }
    }
}
