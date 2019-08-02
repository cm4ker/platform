﻿using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Environment;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Sessions
{
    public class SimpleSession : ISession
    {
        private readonly Dictionary<string, object> _sessionParameters;
        private IDisposable _remover;
        public Guid Id { get; }

        public IUser User { get; }

        public IEnvironment Environment { get; }


        public SimpleSession(IEnvironment env, IUser user)
        {
            Id = Guid.NewGuid();
            User = user;
            Environment = env;
            _sessionParameters = new Dictionary<string, object>();
        }

        public SimpleSession(IUser user) : this(null, user)
        {

        }

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