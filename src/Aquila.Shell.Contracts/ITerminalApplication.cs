using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aquila.Shell.Contracts.Ansi;

namespace Aquila.Shell.Contracts
{
    public interface ITerminalApplication
    {
        void Open(ITerminal terminal);

        void Close();

       

        void Consume(TerminalCode code);

    }
}
