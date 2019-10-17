using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    internal partial class VirtualTerminal : ITerminal
    {
        public void Consume(TerminalCode code)
        {
            CurrentActive.Consume(code);
        }

        public void SetSize(TerminalSize newSize)
        {
            _size = newSize;
            CurrentActive.SetSize(newSize);
        }

        public ExtendedStack<ITerminalApplication> Apps => _apps;

        public ITerminalApplication CurrentActive => (Apps.TryPeek(out var app)) ? app : null;

        public void Close(ITerminalApplication app)
        {
            lock (_apps)
            {
                if (CurrentActive == app)
                {
                    _apps.Remove(app);
                }
            }
        }

        public void Open(ITerminalApplication app)
        {
            lock (_apps)
            {
                if (CurrentActive == app) return;
                Apps.Push(app);
                app.Open(_size);
            }
        }

        private void Send(byte[] data)
        {
            OnData.Invoke(this, data);
        }

        public void CursorPositionRequest()
        {
            Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.DeviceStatusRequest)));
        }
    }
}