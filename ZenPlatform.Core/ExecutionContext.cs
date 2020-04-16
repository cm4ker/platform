using System;
using System.Threading;
using Mono.Cecil;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Environment;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;

namespace ZenPlatform.Core
{
    public class PlatformContext
    {
        public PlatformContext(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }

        public string UserName => Session.User.Name;

        public DataContext DataContext => Session.DataContext;

        public bool IsTypeManagerAvaliable => Session.Environment is IPlatformEnvironment;

        public ITypeManager TypeManager => Environment?.;
        
        public IPlatformEnvironment Environment => Session.Environment as IPlatformEnvironment
    }

    public class ContextHelper
    {
        private static AsyncLocal<PlatformContext> Context = new AsyncLocal<PlatformContext>();

        public static PlatformContext GetContext() => Context.Value;

        public static void SetContext(PlatformContext value) => Context.Value = value;
    }
}