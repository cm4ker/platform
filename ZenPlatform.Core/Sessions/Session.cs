using System.Collections.Generic;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;
using System.Linq;
using System;

namespace ZenPlatform.Core.Sessions
{
    /// <summary>
    /// Абстракция сессии
    /// </summary>
    public abstract class Session : ISession
    {
        protected Session(IEnvironment env, IDataContextManager dataContextManger)
        {
            Environment = env;
            Id = Guid.NewGuid();

            _dataContextManger = dataContextManger;
        }

        private IDisposable _remover;

        public Guid Id { get; }

        protected IDataContextManager _dataContextManger;
        public IEnvironment Environment { get; }
        
        public abstract IUser User { get; protected set; }



        public DataContext DataContext {
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