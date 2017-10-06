using System;
using SqlPlusDbSync.Platform;

namespace SqlPlusDbSync.Transport
{
    public abstract class TransportCommand
    {
        protected TransportCommand(params object[] args)
        {

            Params = new PlatformDynamicObject();
            if (args != null)
                for (int i = 0; i < args.Length; i++)
                {
                    Params[$"arg{i}"] = args[i];
                }

        }

        public abstract string Name { get; }
        public virtual PlatformDynamicObject Params { get; set; }

        public virtual object Execute(Client client)
        {
            throw new NotImplementedException("This command doesn't support execution");
        }
    }

    public abstract class TransportCommand<T> : TransportCommand
    {
        protected TransportCommand(params object[] args) : base(args)
        {
        }

        public virtual T ExecuteCommand(Client client)
        {
            throw new NotImplementedException("This command doesn't support execution");
        }
    }
}