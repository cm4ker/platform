using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Shell.Contracts.Ansi;

namespace ZenPlatform.Shell.Contracts
{
    public interface ITerminalApplication
    {
        void Open(ITerminal terminal);

        void Close();

        void Consume(TerminalCode code);

    }
}
