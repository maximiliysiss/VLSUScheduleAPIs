using Commonlibrary.Models;
using System;
using System.Collections.Generic;

namespace NetServiceConnection.NetContext
{
    public class Transaction
    {
        public string Name { get; set; }
        public Action Action { get; set; }
        public Action Rollback { get; set; }
    }

    public class AddTransaction<T> : Transaction where T : IModel
    {
        private int id;

        public AddTransaction(string address, T item, INetworkModelAccess<T> networkLoad)
        {
            Action = async () =>
            {
                var elem = await networkLoad.Add(address, item);
                id = elem.ID;
            };
            Rollback = () => networkLoad.Delete(address, id);
        }
    }

    public class DeleteTransaction<T> : Transaction where T : IModel
    {
        public DeleteTransaction(string address, T item, INetworkModelAccess<T> networkLoad)
        {
            Action = () => networkLoad.Delete(address, item.ID);
            Rollback = async () =>
            {
                item.ID = 0;
                await networkLoad.Add(address, item);
            };
        }
    }

    public class UpdateTransaction<T> : Transaction where T : IModel
    {
        public UpdateTransaction(string address, T item, T prev, INetworkModelAccess<T> networkLoad)
        {
            Action = () => networkLoad.Put(address, item.ID, item);
            Rollback = () => networkLoad.Put(address, prev.ID, prev);
        }
    }
}