using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace NetServiceConnection.NetContext
{
    public abstract class NetContext
    {
        private readonly IConsulClient consulClient;
        private readonly List<Transaction> transactions = new List<Transaction>();
        private Dictionary<string, string> addresses = new Dictionary<string, string>();

        public readonly string contextTag = "netcontext:";
        public readonly Type type;
        public List<PropertyInfo> propertyInfos;

        public void AddTransaction(Transaction transaction) => transactions.Add(transaction);

        public NetContext(IConsulClient consulClient)
        {
            this.consulClient = consulClient;
            this.type = this.GetType();
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
                            addresses.Add(tagName.ToLower(), service.Value.Address);
                    }
                }
            }

            propertyInfos = this.type.GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType == typeof(NetSet<>)).ToList();
            foreach (var property in propertyInfos)
            {
                try
                {
                    property.SetValue(this, Activator.CreateInstance(property.PropertyType, this, addresses[property.Name.ToLower()]));
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
