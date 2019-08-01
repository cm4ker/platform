using System;
using ServiceStack.Redis;

namespace ZenPlatform.Core.CacheService
{
    public interface ICacheService
    {
        void Set(int databaseId, int typeId, Guid entityId, object value);
        T Get<T>(int databaseId, int typeId, Guid entityId);
    }

    public class CacheService : ICacheService, IDisposable
    {
        private RedisManagerPool _manager;
        private IRedisClient _client;

        public CacheService()
        {
            //TODO: передать параметры подключения к серверу кэширования
            _manager = new RedisManagerPool("localhost:6379");
            _client = _manager.GetClient();
        }

        public void Set(int databaseId, int typeId, Guid entityId, object value)
        {
            var key = GetKey(databaseId, typeId, entityId);
            _client.Set(key, value);
        }

        public T Get<T>(int databaseId, int typeId, Guid entityId)
        {
            var key = GetKey(databaseId, typeId, entityId);
            return _client.Get<T>(key);
        }

        private string GetKey(int databaseId, int typeId, Guid entityId)
        {
            return $"{databaseId}_{typeId}_{entityId}";
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}