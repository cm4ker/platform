using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Aquila.Core.CacheService
{
    public interface ICacheService
    {
        void Set(int databaseId, int typeId, Guid entityId, object value);

        T Get<T>(int databaseId, int typeId, Guid entityId);

        object Get(Type type, int databaseId, int typeId, Guid entityId);
    }

    public class CacheService : ICacheService
    {
        private MemoryCache _cache;
        private int _secondsCount;

        public CacheService()
        {
            _cache = new MemoryCache("AppCache");
            _secondsCount = 15;
            //TODO: передать параметры подключения к серверу кэширования
        }

        public CacheService(int secondsCount) : this()
        {
            _secondsCount = secondsCount;
        }

        public void Set(int databaseId, int typeId, Guid entityId, object value)
        {
            var key = GetKey(databaseId, typeId, entityId);

            var i = new CacheItemPolicy();
            i.AbsoluteExpiration = DateTime.Now.AddSeconds(_secondsCount);
            _cache.AddOrGetExisting(key, value, i);
        }

        public T Get<T>(int databaseId, int typeId, Guid entityId)
        {
            return (T)Get(typeof(T), databaseId, typeId, entityId);
        }

        public object Get(Type type, int databaseId, int typeId, Guid entityId)
        {
            var key = GetKey(databaseId, typeId, entityId);
            return _cache.Get(key);
        }

        private string GetKey(int databaseId, int typeId, Guid entityId)
        {
            return $"{databaseId}_{typeId}_{entityId}";
        }
    }
}