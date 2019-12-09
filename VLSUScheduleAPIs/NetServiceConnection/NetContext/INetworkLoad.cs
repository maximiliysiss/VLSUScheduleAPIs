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
        List<T> Load(string address);
        T Add(string address, T item);
        void Delete(string address, int id);
        T Put(string address, int id, ref T item);
        T Get(string address, int id);
    }

    public class HttpLoad<T> : INetworkModelAccess<T>
    {
        public HttpLoad()
        {
            PreHeader.Add(x => x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")));
        }

        public List<Action<HttpClient>> PreHeader { get; set; } = new List<Action<HttpClient>>();
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
            var model = JsonConvert.DeserializeObject<List<T>>(data) ?? new List<T>();
            foreach (var modelWork in ModelWorker)
                foreach (var m in model)
                    modelWork(m);
            return model;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            foreach (var pre in PreHeader)
                pre(httpClient);
            return httpClient;
        }

        public T Add(string address, T item)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = httpClient.PostAsync(address, new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json")).Result;
                item = GetModel(response.Content.ReadAsStringAsync().Result);
                return item;
            }
        }

        public async void Delete(string address, int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync($"{address}/{id}");
            }
        }

        public T Get(string address, int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = httpClient.GetAsync($"{address}/{id}").Result;
                return GetModel(response.Content.ReadAsStringAsync().Result);
            }
        }

        public List<T> Load(string address)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = httpClient.GetAsync(address).Result;
                return GetListModel(response.Content.ReadAsStringAsync().Result);
            }
        }

        public T Put(string address, int id, ref T item)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = httpClient.PutAsync($"{address}/{id}", new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json")).Result;
                item = GetModel(response.Content.ReadAsStringAsync().Result);
                return item;
            }
        }
    }
}
