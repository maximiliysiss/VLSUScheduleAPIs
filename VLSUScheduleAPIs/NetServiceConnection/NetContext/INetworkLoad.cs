using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetServiceConnection.NetContext
{
    public interface INetworkConstructor
    {
        List<Action<HttpClient>> PreHeader { get; set; }
        List<Action<object>> ModelWorker { get; set; }
    }

    public interface INetworkModelAccess<T> : INetworkConstructor
    {
        Task<List<T>> Load(string address);
        Task<T> Add(string address, T item);
        void Delete(string address, int id);
        Task<T> Put(string address, int id, T item);
        Task<T> Get(string address, int id);
    }

    public class HttpLoad<T> : INetworkModelAccess<T>
    {

        public List<Action<HttpClient>> PreHeader { get; set; }
        public List<Action<object>> ModelWorker { get; set; } = new List<Action<object>>();

        private T GetModel(string data)
        {
            var model = JsonConvert.DeserializeObject<T>(data);
            foreach (var modelWork in ModelWorker)
                modelWork(model);
            return model;
        }

        private List<T> GetListModel(string data)
        {
            var model = JsonConvert.DeserializeObject<List<T>>(data);
            foreach (var modelWork in ModelWorker)
                foreach (var m in model)
                    modelWork(m);
            return model;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            if (PreHeader != null)
            {
                foreach (var pre in PreHeader)
                    pre(httpClient);
            }
            return httpClient;
        }

        public async Task<T> Add(string address, T item)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PostAsync(address, new StringContent(JsonConvert.SerializeObject(item)));
                return GetModel(await response.Content.ReadAsStringAsync());
            }
        }

        public async void Delete(string address, int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync($"{address}/{id}");
            }
        }

        public async Task<T> Get(string address, int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"{address}/{id}");
                return GetModel(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<List<T>> Load(string address)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync(address);
                return GetListModel(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<T> Put(string address, int id, T item)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PutAsync($"{address}/{id}", new StringContent(JsonConvert.SerializeObject(item)));
                return GetModel(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
