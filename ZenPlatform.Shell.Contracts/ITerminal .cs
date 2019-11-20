using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Shell.Contracts.Ansi;

namespace ZenPlatform.Shell.Contracts
{
    public interface ITerminal
    {
        /// <summary>
        /// Потребить данные
        /// </summary>
        /// <param name="code">Терминальный код</param>
        void Consume(TerminalCode code);

        TerminalSize Size { get; set; }

        event EventHandler<byte[]> OnData;

        void Send(byte[] data);

        void Initialize(ITerminalApplication application);

    }
}
