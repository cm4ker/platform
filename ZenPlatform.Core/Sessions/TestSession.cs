using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Sessions
{
    public class TestSession : ISession
    {
        private readonly Dictionary<string, object> _sessionParameters;
        private IDisposable _remover;
        public Guid Id { get; }

        public IUser User { get; }

        public DataContext DataContext => throw new NotImplementedException();

        public IEnvironment Environment { get; }

        public TestSession(IEnvironment env, IUser user)
        {
            Id = Guid.NewGuid();
            User = user;
            Environment = env;
            _sessionParameters = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            _remover.Dispose();
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
    }
}
