using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
namespace ZenPlatform.ServerClientShared.DI
{
    public class MicrosoftDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Resolve<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public IDependencyScope BeginScope()
        {
            return new MicrosoftDependencyScope(_serviceProvider.CreateScope());
        }
    }
}
