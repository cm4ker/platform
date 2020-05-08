using System.Collections.Generic;
using Aquila.Core.Authentication;
using Aquila.Core.Environment;
using Aquila.Data;
using System.Linq;
using System;
using Aquila.Core.CacheService;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Environment;

namespace Aquila.Core.Sessions
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

        private IDisposable _remover;

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