using System;
using Aquila.Shell.Terminal;
using Aquila.SSH;

namespace Aquila.Shell.MiniTerm
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