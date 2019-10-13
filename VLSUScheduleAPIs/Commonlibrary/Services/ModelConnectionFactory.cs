using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Commonlibrary.Services
{
    public class ModelConnectionFactory
    {
        private readonly ILogger logger;

        public ModelConnectionFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public ModelServiceConnector<T> Build<T>(string uri) where T : class => new ModelServiceConnector<T>(uri, logger);
    }

    public class ModelServiceConnector<T> where T : class
    {
        private string uri;
        private readonly ILogger logger;

        public string Uri
        {
            set => uri = value;
        }

        public ModelServiceConnector(string uri, ILogger logger)
        {
            this.uri = uri;
            this.logger = logger;
        }

        public async Task<T> Create(T elem)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(elem));
                    var res = await client.PostAsync(uri, content);
                    return JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return null;
        }
    }
}
