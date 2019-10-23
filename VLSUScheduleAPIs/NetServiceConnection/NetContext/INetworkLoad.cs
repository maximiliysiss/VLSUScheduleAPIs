using NetServiceConnection.Exception;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetServiceConnection.NetContext
{
    public interface INetworkAccess<T>
    {
        Task<List<T>> Load(string address);
        Task<T> Add(string address, T item);
        void Delete(string address, int id);
        Task<T> Put(string address, int id, T item);
        Task<T> Get(string address, int id);
        List<Action<HttpClient>> PreHeader { get; set; }
    }

    public class HttpLoad<T> : INetworkAccess<T>
    {
        public List<Action<HttpClient>> PreHeader { get; set; }

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
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
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
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<List<T>> Load(string address)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync(address);
                return JsonConvert.DeserializeObject<List<T>>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<T> Put(string address, int id, T item)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.PutAsync($"{address}/{id}", new StringContent(JsonConvert.SerializeObject(item)));
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
