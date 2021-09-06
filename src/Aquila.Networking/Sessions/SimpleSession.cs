using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Instance;
using Aquila.Data;
using Aquila.QueryBuilder;

namespace Aquila.Core.Sessions
{
    public class SimpleSession : ISession
    {
        private readonly Dictionary<string, object> _sessionParameters;
        private IDisposable _remover;
        private readonly DataContextManager _dataContextManger;
        private readonly DataConnectionContext _dataContext;

        public SimpleSession(IPlatformInstance env, IUser user)
        {
            Id = Guid.NewGuid();
            User = user;
            Instance = env;

            _sessionParameters = new Dictionary<string, object>();
            _dataContextManger = env.DataContextManager;
            _dataContext = _dataContextManger.GetContext();
        }

        public Guid Id { get; }

        public IUser User { get; }

        public DataConnectionContext DataContext
        {
            get => _dataContext;
        }

        public IPlatformInstance Instance { get; }


        public void Dispose()
        {
            Close();
        }

        public object GetSessionParameter(string key, object value)
        {
            if (_sessionParameters.TryGetValue(key, out var result))
                return result;
            else
                return null;
        }

        public void SetRemover(IDisposable remover)
        {
            _remover = remover;
        }

        public void SetSessionParameter(string key, object value)
        {
            _sessionParameters[key] = value;
        }

        public void Close()
        {
            _remover?.Dispose();
        }
    }
}