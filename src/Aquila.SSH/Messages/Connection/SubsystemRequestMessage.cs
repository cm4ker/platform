using System.Text;

namespace Aquila.SSH.Messages.Connection
{
    public class SubsystemRequestMessage : ChannelRequestMessage
    {
        public string Name { get; private set; }

        protected override void OnLoad(SshDataWorker reader)
        {
            base.OnLoad(reader);

            Name = reader.ReadString(Encoding.ASCII);
        }
    }
}
