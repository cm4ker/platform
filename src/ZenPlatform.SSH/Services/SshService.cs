using System.Diagnostics.Contracts;

namespace ZenPlatform.SSH.Services
{
    public abstract class SshService
    {
        protected internal readonly Session _session;

        public SshService(Session session)
        {
            Contract.Requires(session != null);

            _session = session;
        }

        internal protected abstract void CloseService();
    }
}
