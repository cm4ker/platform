using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.DI
{
    public interface IDependencyScope: IDisposable
    {
        IDependencyResolver Resolver { get; }
    }
}
