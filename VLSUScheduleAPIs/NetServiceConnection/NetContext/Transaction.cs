using System;
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

    public class AddTransaction<T> : ModelTransaction
    {
        private static readonly PropertyInfo idProperty;

        static AddTransaction()
        {
            idProperty = typeof(T).GetProperty("ID");
        }

        private int id;

        public AddTransaction(string address, T item, INetworkModelAccess<T> networkLoad)
        {
            Action = () =>
            {
                var elem = networkLoad.Add(address, item);
                id = (int)idProperty.GetValue(item);
                idProperty.SetValue(item, id);
            };
            Rollback = () => networkLoad.Delete(address, id);
        }
    }

    public class DeleteTransaction<T> : ModelTransaction
    {
        private static readonly PropertyInfo idProperty;

        static DeleteTransaction()
        {
            idProperty = typeof(T).GetProperty("ID");
        }
        public DeleteTransaction(string address, T item, INetworkModelAccess<T> networkLoad)
        {
            Action = () => networkLoad.Delete(address, (int)GetIdProperty.GetValue(item));
            Rollback = () =>
            {
                idProperty.SetValue(item, 0);
                networkLoad.Add(address, ref item);
            };
        }
    }

    public class UpdateTransaction<T> : ModelTransaction
    {
        private static readonly PropertyInfo idProperty;

        static UpdateTransaction()
        {
            idProperty = typeof(T).GetProperty("ID");
        }

        public UpdateTransaction(string address, T item, T prev, INetworkModelAccess<T> networkLoad)
        {
            Action = () => networkLoad.Put(address, (int)idProperty.GetValue(item), ref item);
            Rollback = () => networkLoad.Put(address, (int)idProperty.GetValue(item), ref prev);
        }
    }
}