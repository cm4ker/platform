using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ZenPlatform.Core.CacheService
{
    public class DictionaryCacheService : ICacheService
    {
        private class CacheUnit
        {
            public object cache;
            public DateTime ttl;
        }
        private ConcurrentDictionary<string, CacheUnit> _cache;

        private Timer _timer;

        public DictionaryCacheService()
        {
            _cache = new ConcurrentDictionary<string, CacheUnit>();
            _timer = new Timer(new TimerCallback(ClearCache), null, 0, 10*60*1000); //раз в 10 минут удаляем все что в кеше завалялось
        }

        private void ClearCache(object obj)
        {
            
            var now = DateTime.Now;
            foreach (var check in _cache)
            {
                if ((now - check.Value.ttl).TotalMinutes > 2)
                    _cache.TryRemove(check.Key, out _);
            }
        }

        public T Get<T>(int databaseId, int typeId, Guid entityId)
        {
            return (T)Get(typeof(T), databaseId, typeId, entityId);
        }

        public object Get(Type type, int databaseId, int typeId, Guid entityId)
        {
            var key = GetKey(databaseId, typeId, entityId);
            if (_cache.ContainsKey(key))
            {
                var result = _cache[key];
                if (((result.ttl - DateTime.Now).TotalMinutes < 2) &&
                    (result.cache.GetType().Equals(type)))
                {
                    result.ttl = DateTime.Now;
                    return result.cache;
                }
                else
                {
                    _cache.TryRemove(key, out _);
                    return null;
                }
            }
            return null;
        }

        public void Set(int databaseId, int typeId, Guid entityId, object value)
        {
            var key = GetKey(databaseId, typeId, entityId);
            var cacheUnit = new CacheUnit() { cache = value, ttl = DateTime.Now };
            _cache.AddOrUpdate(key, cacheUnit, (key, existing) => cacheUnit);
        }
        private string GetKey(int databaseId, int typeId, Guid entityId)
        {
            return $"{databaseId}_{typeId}_{entityId}";
        }

    }
}
