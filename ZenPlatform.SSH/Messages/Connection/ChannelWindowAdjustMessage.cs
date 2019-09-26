
namespace FxSsh.Messages.Connection
{
    [Message("SSH_MSG_CHANNEL_WINDOW_ADJUST", MessageNumber)]
    public class ChannelWindowAdjustMessage : ConnectionServiceMessage
    {
        private const byte MessageNumber = 93;

        public uint RecipientChannel { get; set; }
        public uint BytesToAdd { get; set; }

        public override byte MessageType { get { return MessageNumber; } }

        protected override void OnLoad(SshDataWorker reader)
        {
            RecipientChannel = reader.ReadUInt32();
            BytesToAdd = reader.ReadUInt32();
        }

        protected override void OnGetPacket(SshDataWorker writer)
        {
            writer.Write(RecipientChannel);
            writer.Write(BytesToAdd);
        }
    }


    public struct ConsoleSize
    {

        public uint WidthColumns { get; set; }
        public uint HeightRows { get; set; }


        public uint WidthPixels { get; set; }
        public uint HeightPixels { get; set; }

    }

    public class ChannelWindowChangeMessage : ChannelRequestMessage
    {
        private const byte MessageNumber = 98;

        public ConsoleSize Size { get; set; }

        public override byte MessageType { get { return MessageNumber; } }

        protected override void OnLoad(SshDataWorker reader)
        {
            base.OnLoad(reader);

            Size = new ConsoleSize()
            {
                WidthColumns = reader.ReadUInt32(),
                HeightRows = reader.ReadUInt32(),

                WidthPixels = reader.ReadUInt32(),
                HeightPixels = reader.ReadUInt32()
            };
        }

        protected override void OnGetPacket(SshDataWorker writer)
        {
            writer.Write(Size.WidthColumns);
            writer.Write(Size.HeightRows);

            writer.Write(Size.WidthPixels);
            writer.Write(Size.HeightPixels);
        }
    }
}
