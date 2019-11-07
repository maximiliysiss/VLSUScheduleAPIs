using Commonlibrary.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NetServiceConnection.NetContext
{
    public class ModelTransaction
    {
        public string Name { get; set; }
        public Action Action { get; set; }
        public Action Rollback { get; set; }
        public PropertyInfo GetIdProperty { get; set; }
    }

    public class AddTransaction<T> : ModelTransaction where T : IModel
    {
        private int id;

        public AddTransaction(string address, T item, INetworkModelAccess<T> networkLoad)
        {
            Action = () =>
            {
                var elem = networkLoad.Add(address, ref item);
                id = elem.ID;
                item.ID = id;
            };
            Rollback = () => networkLoad.Delete(address, id);
        }
    }

    public class DeleteTransaction<T> : ModelTransaction where T : IModel
    {
        public DeleteTransaction(string address, T item, INetworkModelAccess<T> networkLoad)
        {
            Action = () => networkLoad.Delete(address, (int)GetIdProperty.GetValue(item));
            Rollback = () =>
            {
                item.ID = 0;
                networkLoad.Add(address, ref item);
            };
        }
    }

    public class UpdateTransaction<T> : ModelTransaction where T : IModel
    {
        public UpdateTransaction(string address, T item, T prev, INetworkModelAccess<T> networkLoad)
        {
            Action = () => networkLoad.Put(address, item.ID, ref item);
            Rollback = () => networkLoad.Put(address, prev.ID, ref prev);
        }
    }
}