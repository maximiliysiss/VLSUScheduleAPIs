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
        List<Transaction> Transactions { get; set; }
    }

    public class NetSet<T> : IEnumerable<T>, ITransactionContains where T : IModel
    {
        private readonly string address;
        private readonly INetworkAccess<T> networkLoad;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public NetSet(string address, INetworkAccess<T> networkLoad)
        {
            this.address = address;
            this.networkLoad = networkLoad;
        }

        protected async Task<List<T>> List() => await networkLoad.Load(address);

        public IEnumerator<T> GetEnumerator() => List().Result.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => Transactions.Add(new AddTransaction<T>(address, item, networkLoad));

        public void Update(T item)
        {
            var prev = networkLoad.Get(address, item.ID).Result;
            Transactions.Add(new UpdateTransaction<T>(address, item, prev, networkLoad));
        }

        public void Remove(T item) => Transactions.Add(new DeleteTransaction<T>(address, item, networkLoad));
    }
}
