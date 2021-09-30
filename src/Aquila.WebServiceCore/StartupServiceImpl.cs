using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Aquila.Logging;

namespace Aquila.WebServiceCore
{
    public class StartupServiceImpl
    {
        private readonly ILogger<StartupServiceImpl> _logger;

        private List<Action<IApplicationBuilder>> _listActions = new List<Action<IApplicationBuilder>>();

        private List<Type> _types = new List<Type>();

        public StartupServiceImpl(ILogger<StartupServiceImpl> logger)
        {
            _logger = logger;
        }

        public void Register(Action<IApplicationBuilder> a)
        {
            _listActions.Add(a);

            _logger.Trace("Register new service method, see next instruction for information");
        }

        public void RegisterWebServiceClass<T>() where T : class
        {
            _logger.Trace($"Register new service, type: {typeof(T).Name}");
            _types.Add(typeof(T));
        }

        public void Configure(IApplicationBuilder buildedr)
        {
            foreach (var action in _listActions)
            {
                action(buildedr);
            }
        }

        public void ConfigureServices(IServiceCollection sc)
        {
            foreach (var type in _types)
            {
                sc.AddSingleton(type);
            }
        }
    }
}