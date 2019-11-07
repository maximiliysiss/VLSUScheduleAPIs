using Commonlibrary.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetServiceConnection.NetContext
{
    public interface ITransactionContains
    {
        List<ModelTransaction> Transactions { get; set; }
    }

    public class NetSet<T> : IEnumerable<T>, ITransactionContains where T : IModel
    {
        private readonly string address;
        private readonly INetworkModelAccess<T> networkLoad;
        public List<ModelTransaction> Transactions { get; set; } = new List<ModelTransaction>();

        public NetSet(string address, INetworkModelAccess<T> networkLoad)
        {
            this.address = address;
            this.networkLoad = networkLoad;
        }

        protected List<T> List() => networkLoad.Load(address);

        public IEnumerator<T> GetEnumerator() => List().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => Transactions.Add(new AddTransaction<T>(address, item, networkLoad));

        public void Update(T item)
        {
            var prev = networkLoad.Get(address, item.ID);
            Transactions.Add(new UpdateTransaction<T>(address, item, prev, networkLoad));
        }

        public T Get(int id) => networkLoad.Get(address, id);

        public void Remove(T item) => Transactions.Add(new DeleteTransaction<T>(address, item, networkLoad));
    }
}
