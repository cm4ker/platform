using System;
using System.Threading;
using Mono.Cecil;
using ZenPlatform.Core.Sessions;

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
    }

    public class ContextHelper
    {
        private static AsyncLocal<PlatformContext> Context = new AsyncLocal<PlatformContext>();


        public static PlatformContext GetContext() => Context.Value;

        public static void SetContext(PlatformContext value) => Context.Value = value;
    }
}