using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using tterm.Ansi;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    internal partial class VirtualTerminal : ITerminal, IConsole
    {
        /// <summary>
        /// Индекс начиначется с 0
        /// </summary>
        public int CursorX => _cursorX;

        /// <summary>
        /// Индекс начиначется с 1
        /// </summary>
        public int CursorY => _cursorY;

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

        public void SetCursorPosition(int x, int y)
        {
            Send(AnsiBuilder.SetCursorPosCommand(y, x));
        }

        public void CursorPositionRequest()
        {
            Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.DeviceStatusRequest)));
        }

        public void WriteLine(string text = "")
        {
            Write(text);
            Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.LineFeed)));
        }

        public void Write(string text)
        {
            Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.Text, text)));
        }
    }
}