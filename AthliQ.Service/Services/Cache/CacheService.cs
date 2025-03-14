using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AthliQ.Core.Service.Contract;
using StackExchange.Redis;

namespace AthliQ.Service.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _database;

        public CacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<string> GetCacheKeyAsync(string key)
        {
            var cachResponse = await _database.StringGetAsync(key);
            if (cachResponse.IsNullOrEmpty)
                return null;

            return cachResponse.ToString();
        }

        public async Task SetCacheKeyAsync(string key, object response, TimeSpan expireTime)
        {
            if (response is null)
                return;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            await _database.StringSetAsync(
                key,
                JsonSerializer.Serialize(response, jsonOptions),
                expireTime
            );
        }
    }
}
