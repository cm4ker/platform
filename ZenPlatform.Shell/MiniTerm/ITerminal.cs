using System;
using ZenPlatform.Shell.Terminal;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.MiniTerm
{
    public interface ITerminal : IDisposable
    {
        event EventHandler<uint> CloseReceived;
        event EventHandler<byte[]> DataReceived;

        void OnClose();
        void OnInput(byte[] data);
        void OnSizeChanged(TerminalSize size);
        void Run();
    }
}