using System;

namespace SqlPlusDbSync.Transport
{
    public class GreetingCommand : TransportCommand<Guid>
    {
        /// <summary>
        /// arg0 = guid PointId
        /// </summary>
        /// <param name="args"></param>
        public GreetingCommand(params object[] args) : base(args)
        {

        }

        public Guid PointId => (Guid)Params["arg0"];
        public override string Name => nameof(GreetingCommand);

        public override Guid ExecuteCommand(Client client)
        {
            return PointId;
        }

        public override object Execute(Client client)
        {
            return PointId;
        }
    }

    public class GreetingResponceCommand : TransportCommand<Guid>
    {
        /// <summary>
        /// arg0 = guid PointId
        /// </summary>
        /// <param name="args"></param>
        public GreetingResponceCommand(params object[] args) : base(args)
        {

        }

        public Guid PointId => (Guid)Params["arg0"];
        public override string Name => nameof(GreetingCommand);

        public override Guid ExecuteCommand(Client client)
        {
            return PointId;
        }

        public override object Execute(Client client)
        {
            return PointId;
        }
    }
}