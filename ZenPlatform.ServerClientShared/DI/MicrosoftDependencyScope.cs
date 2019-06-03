using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.DI
{
    public class MicrosoftDependencyScope: IDependencyScope
    {
        private IServiceScope _scope;
        public MicrosoftDependencyScope(IServiceScope scope)
        {
            _scope = scope;
        }

        public IDependencyResolver Resolver { get => new MicrosoftDependencyResolver(_scope.ServiceProvider); }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
