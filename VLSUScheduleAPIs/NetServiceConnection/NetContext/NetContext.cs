using Consul;
using NetServiceConnection.NetAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace NetServiceConnection.NetContext
{
    public abstract class NetContext
    {
        private readonly IConsulClient consulClient;
        private readonly Dictionary<string, string> addresses = new Dictionary<string, string>();
        private readonly IServiceProvider serviceProvider;
        public List<Action<HttpClient>> preHeader = new List<Action<HttpClient>>();
        public bool isLazy = false;

        public static readonly string contextTag = "netcontext:";
        public readonly Type type;
        public List<PropertyInfo> propertyInfos;

        private Action<object> CreateLazy(Type type)
        {
            var props = type.GetProperties().Select(x => new { Prop = x, Attr = x.GetCustomAttributes(typeof(LazyNetLoad), false).OfType<LazyNetLoad>().ToArray() })
                .Where(x => x.Attr.Length > 0 && addresses.ContainsKey(x.Attr[0].Service));
            return (x) =>
            {
                foreach (var prop in props)
                {
                    var lazyData = prop.Attr[0];
                    var propLazy = type.GetProperty(lazyData.Id);
                    var netSet = CreateNetSet(lazyData.Service, propLazy.PropertyType);
                    var getMethod = netSet.GetType().GetMethod("Get");
                    propLazy.SetValue(x, getMethod.Invoke(netSet, new[] { prop.Prop.GetValue(x) }));
                }
            };
        }

        public NetContext(IConsulClient consulClient, IServiceProvider serviceProvider)
        {
            this.consulClient = consulClient;
            this.type = this.GetType();
            this.serviceProvider = serviceProvider;

            OnConfiguration();
        }

        public object CreateNetSet(string name, Type type)
        {
            var genericType = typeof(INetworkModelAccess<>);
            var networkService = serviceProvider.GetService(genericType.MakeGenericType(type));
            var networkConstructor = networkService as INetworkConstructor;
            if (isLazy)
                networkConstructor.ModelWorker.Add(CreateLazy(type));
            networkConstructor.PreHeader.AddRange(preHeader);
            return Activator.CreateInstance(typeof(NetSet<>).MakeGenericType(type), addresses[name.ToLower()], networkService);
        }

        public async virtual void OnConfiguration()
        {
            var services = await consulClient.Agent.Services();
            foreach (var service in services.Response)
            {
                foreach (var tag in service.Value.Tags)
                {
                    if (Regex.IsMatch(tag, $"^{contextTag}.+$"))
                    {
                        var tagName = tag.Substring(tag.IndexOf(contextTag) + contextTag.Length);
                        if (!addresses.ContainsKey(tagName))
                            addresses.Add(tagName.ToLower(), $"{service.Value.Address}/api/{tagName}");
                    }
                }
            }

            propertyInfos = this.type.GetProperties()
                .Where(x => x.PropertyType.IsGenericType && x.PropertyType.Assembly.FullName == typeof(NetSet<>).Assembly.FullName).ToList();
            foreach (var property in propertyInfos)
            {
                try
                {
                    property.SetValue(this, CreateNetSet(property.Name, property.PropertyType.GetGenericArguments()[0]));
                }
                catch (System.Exception)
                {
                }
            }

        }

        public void Commit()
        {
            foreach (var property in propertyInfos)
            {
                int i = 0;
                var prop = property.GetValue(this) as ITransactionContains;
                try
                {
                    for (i = 0; i < prop.Transactions.Count; i++)
                        prop.Transactions[i].Action();
                }
                catch (System.Exception)
                {
                    for (int j = 0; j < i; j++)
                        prop.Transactions[j].Rollback();
                }
                finally
                {
                    prop.Transactions.Clear();
                }
            }
        }
    }
}
