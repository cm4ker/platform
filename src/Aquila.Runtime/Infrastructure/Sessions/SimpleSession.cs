﻿using System;
using System.Collections.Generic;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Instance;
using Aquila.Data;

namespace Aquila.Core.Sessions
{
    public class SimpleSession : ISession
    {
        private readonly Dictionary<string, object> _sessionParameters;
        private IDisposable _remover;
        private readonly DataContextManager _dataContextManger;
        private readonly DataConnectionContext _dataContext;

        public SimpleSession(AqInstance instance, AqUser user)
        {
            Id = Guid.NewGuid();
            User = user;
            Instance = instance;

            _sessionParameters = new Dictionary<string, object>();
            _dataContextManger = instance.DataContextManager;
            _dataContext = _dataContextManger.GetContext();
        }

        public Guid Id { get; }

        public AqUser User { get; }

        public DataConnectionContext DataContext
        {
            get => _dataContext;
        }

        public AqInstance Instance { get; }


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