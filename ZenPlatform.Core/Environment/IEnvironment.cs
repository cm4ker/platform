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
    public interface IEnvironment
    {
        XCRoot Configuration { get; }
        IList<ISession> Sessions { get; }
        IInvokeService InvokeService { get; }
        void Initialize(StartupConfig config);
        IAuthenticationManager AuthenticationManager { get; }

        EntityMetadata GetMetadata(Guid key);
        EntityMetadata GetMetadata(Type type);
        IEntityManager GetManager(Type type);
        ISession CreateSession(IUser user);

        IDataContextManager DataContextManager { get; }
    }
}