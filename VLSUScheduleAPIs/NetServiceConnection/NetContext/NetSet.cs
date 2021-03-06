﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NetServiceConnection.NetContext
{
    public interface ITransactionContains
    {
        List<ModelTransaction> Transactions { get; set; }
    }

    public class NetSet<T> : IEnumerable<T>, ITransactionContains
    {
        private static readonly PropertyInfo idProperty;

        static NetSet()
        {
            idProperty = typeof(T).GetProperty("ID");
        }

        private readonly string name;
        private string address;
        public string Address
        {
            get
            {
                if (string.IsNullOrEmpty(address))
                    address = parent.GetAddress(name).Result;
                return address;
            }
        }
        private readonly NetContext parent;
        private readonly INetworkModelAccess<T> networkLoad;
        public List<ModelTransaction> Transactions { get; set; } = new List<ModelTransaction>();

        public NetSet(string name, string address, INetworkModelAccess<T> networkLoad, NetContext netContext)
        {
            this.name = name;
            this.address = address;
            this.parent = netContext;
            this.networkLoad = networkLoad;
        }

        protected List<T> List() => networkLoad.Load(address);

        public IEnumerator<T> GetEnumerator() => List().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => Transactions.Add(new AddTransaction<T>(address, item, networkLoad));

        public void Update(T item)
        {
            var prev = networkLoad.Get(address, (int)idProperty.GetValue(item));
            Transactions.Add(new UpdateTransaction<T>(address, item, prev, networkLoad));
        }

        public T Get(int id) => networkLoad.Get(address, id);

        public void Remove(T item) => Transactions.Add(new DeleteTransaction<T>(address, item, networkLoad));
    }
}
