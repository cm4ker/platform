using Aquila.Data;
using System;
using Aquila.Core.Authentication;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Instance;

namespace Aquila.Core.Sessions
{
    /// <summary>
    /// Абстракция сессии
    /// </summary>
    public abstract class Session : ISession
    {
        protected Session(AqInstance instance, ICacheService cacheService)
        {
            Instance = instance;
            Id = Guid.NewGuid();
            CacheService = cacheService;
            _dataContextManger = instance.DataContextManager;
        }

        private IDisposable _remover;

        public Guid Id { get; }

        protected DataContextManager _dataContextManger;

        public AqInstance Instance { get; }

        public abstract AqUser User { get; protected set; }

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