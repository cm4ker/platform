using System.Collections.Generic;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;
using System.Linq;
using System;
using ZenPlatform.Core.CacheService;

namespace ZenPlatform.Core.Sessions
{
    /// <summary>
    /// Абстракция сессии
    /// </summary>
    public abstract class Session : ISession
    {
        protected Session(IEnvironment env, IDataContextManager dataContextManger, ICacheService cacheService)
        {
            Environment = env;
            Id = Guid.NewGuid();
            CacheService = cacheService;
            _dataContextManger = dataContextManger;
        }

        public Guid Id { get; }

        protected IDataContextManager _dataContextManger;

        public IEnvironment Environment { get; }

        public abstract IUser User { get; protected set; }

        public ICacheService CacheService { get; }

        public DataContext DataContext
        {
            get => _dataContextManger.GetContext();
        }


        public abstract void SetSessionParameter(string key, object value);
        public abstract object GetSessionParameter(string key, object value);
    }
}