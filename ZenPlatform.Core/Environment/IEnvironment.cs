using System.Collections.Generic;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Authentication;
using ZenPlatform.QueryBuilder;
using System;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Environment
{

    public interface IAdminEnvironment : IEnvironment { }
    public interface ITestEnvironment : IEnvironment { }
    public interface IWorkEnvironment : IEnvironment { }
    public interface IEnvironment
    {
        string Name { get; }
        IList<ISession> Sessions { get; }
        IInvokeService InvokeService { get; }
        void Initialize(StartupConfig config);
        IAuthenticationManager AuthenticationManager { get; }

        ISession CreateSession(IUser user);

    }
}