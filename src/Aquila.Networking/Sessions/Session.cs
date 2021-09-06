using System.Collections.Generic;
using Aquila.Core.Authentication;
using Aquila.Core.Instance;
using Aquila.Data;
using System.Linq;
using System;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;

namespace Aquila.Core.Sessions
{
    /// <summary>
    /// Абстракция сессии
    /// </summary>
    public abstract class Session : ISession
    {
        protected Session(IPlatformInstance env, ICacheService cacheService)
        {
            Instance = env;
            Id = Guid.NewGuid();
            CacheService = cacheService;
            _dataContextManger = env.DataContextManager;
        }

        private IDisposable _remover;

        public Guid Id { get; }

        protected DataContextManager _dataContextManger;

        public IPlatformInstance Instance { get; }

        public abstract IUser User { get; protected set; }

        public ICacheService CacheService { get; }

        public DataConnectionContext DataContext
        {
            get => _dataContextManger.GetContext();
        }


        public abstract void SetSessionParameter(string key, object value);
        public abstract object GetSessionParameter(string key, object value);

        public void SetRemover(IDisposable remover)
        {
            _remover = remover;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            _remover?.Dispose();
        }
    }
}