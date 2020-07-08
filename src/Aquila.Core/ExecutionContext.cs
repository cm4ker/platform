using System;
using System.Threading;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Environment;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Core.Environment;
using Aquila.Core.Sessions;
using Aquila.Data;

namespace Aquila.Core
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