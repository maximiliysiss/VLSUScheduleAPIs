using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VLSUScheduleAPIs.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;
        private ConnectionMultiplexer _redis;
        private readonly ILogger<RedisService> logger;

        public RedisService(IConfiguration config, ILogger<RedisService> logger)
        {
            _redisHost = config["Redis:Host"];
            _redisPort = Convert.ToInt32(config["Redis:Port"]);
            this.logger = logger;
        }

        public void Connect()
        {
            try
            {
                var configString = $"{_redisHost}:{_redisPort},connectRetry=5";
                _redis = ConnectionMultiplexer.Connect(configString);
            }
            catch (RedisConnectionException err)
            {
                logger.LogError(err.ToString());
                throw err;
            }
            logger.LogDebug("Connected to Redis");
        }

        public bool Set(string key, string value)
        {
            var db = _redis.GetDatabase();
            return db.StringSet(key, value);
        }

        public string Get(string key)
        {
            var db = _redis.GetDatabase();
            return db.StringGet(key);
        }

        public async Task<bool> SetAsync(string key, string value)
        {
            var db = _redis.GetDatabase();
            return await db.StringSetAsync(key, value);
        }

        public async Task<string> GetAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public bool SetObject<T>(string key, T obj, TimeSpan? expiry = null) where T : class
        {
            var db = _redis.GetDatabase();
            return db.StringSet(key, JsonConvert.SerializeObject(obj), expiry);
        }

        public T GetObject<T>(string key) where T : class
        {
            var db = _redis.GetDatabase();
            var strRes = db.StringGet(key);
            if (!strRes.HasValue)
                return null;
            return JsonConvert.DeserializeObject<T>(strRes);
        }

        public List<T> GetObjectByKeysSet<T>(string key) where T : class
        {
            EndPoint endPoint = _redis.GetEndPoints().First();
            RedisKey[] keys = _redis.GetServer(endPoint).Keys(pattern: key).ToArray();
            var db = _redis.GetDatabase();
            return keys.Select(x => JsonConvert.DeserializeObject<T>(db.StringGet(x))).ToList();
        }

        public bool Delete(string key)
        {
            var db = _redis.GetDatabase();
            return db.KeyDelete(key);
        }
    }
}
