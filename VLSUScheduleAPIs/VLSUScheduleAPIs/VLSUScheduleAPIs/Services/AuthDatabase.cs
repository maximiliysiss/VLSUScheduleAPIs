using Commonlibrary.Models;
using Consul;
using NetServiceConnection.NetContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public class AuthNetContext : NetContext
    {
        public AuthNetContext(IConsulClient consulClient, IServiceProvider serviceProvider) : base(consulClient, serviceProvider)
        {
        }

        public NetSet<User> Users { get; set; }
        public NetSet<Student> Students { get; set; }
    }
}
