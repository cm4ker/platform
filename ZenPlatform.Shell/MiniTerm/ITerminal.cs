using System;
using FxSsh.Messages.Connection;

namespace MiniTerm
{
    public interface ITerminal : IDisposable
    {
        event EventHandler<uint> CloseReceived;
        event EventHandler<byte[]> DataReceived;

        void OnClose();
        void OnInput(byte[] data);
        void OnSizeChanged(ConsoleSize size);
        void Run();
    }
}