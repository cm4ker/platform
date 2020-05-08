using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Environment;
using Aquila.Data;

namespace Aquila.Core.Sessions
{
    /// <summary>
    /// Системная сессия, нужна для того, чтобы выполнять операции, которые не может выполнить пользователь
    /// Например обновление схемы данных
    /// </summary>
    public class SystemSession : Session
    {
        public SystemSession(IEnvironment env, IDataContextManager dataContextManger, ICacheService cacheService)
            : base(env, dataContextManger, cacheService)
        {
            User = new SystemUser();
        }

        public override IUser User { get; protected set; }

        public override object GetSessionParameter(string key, object value)
        {
            throw new System.NotImplementedException();
        }

        public override void SetSessionParameter(string key, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}