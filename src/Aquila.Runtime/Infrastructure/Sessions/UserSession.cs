using System.Collections.Concurrent;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Instance;
using Aquila.Data;

namespace Aquila.Core.Sessions
{
    public class UserSession : Session
    {
        private readonly ConcurrentDictionary<string, object> _sessionParameters;

        public UserSession(AqInstance instance, AqUser user, DataContextManager dataContextManger,
            ICacheService cacheService)
            : base(instance, cacheService)
        {
            _sessionParameters = new ConcurrentDictionary<string, object>();
            User = user;
        }

        public override AqUser User { get; protected set; }


        /// <summary>
        /// Установить параметр сессии
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void SetSessionParameter(string key, object value)
        {
            _sessionParameters[key] = value;
        }


        /// <summary>
        /// Получить параметр сесси
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object GetSessionParameter(string key, object value)
        {
            if (_sessionParameters.TryGetValue(key, out var result))
                return result;
            else
                return null;
        }
    }
}