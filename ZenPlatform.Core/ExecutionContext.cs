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

        public bool IsTypeManagerAvailable => Session.Environment is IPlatformEnvironment;

        public bool IsLinkFactoryAvailable => Environment is IWorkEnvironment;

        public ITypeManager TypeManager => Environment?.Configuration.TypeManager;

        public IPlatformEnvironment Environment => Session.Environment as IPlatformEnvironment;

        public ILinkFactory LinkFactory => (Session.Environment as IWorkEnvironment)?.LinkFactory ??
                                           throw new Exception("Work environment is not available");
    }

    public class ContextHelper
    {
        private static AsyncLocal<PlatformContext> Context = new AsyncLocal<PlatformContext>();

        public static PlatformContext GetContext() => Context.Value;

        public static void SetContext(PlatformContext value) => Context.Value = value;
    }
}