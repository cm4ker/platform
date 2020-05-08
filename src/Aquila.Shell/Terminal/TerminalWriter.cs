using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Aquila.Shell.Ansi;
using Aquila.Shell.Contracts;

namespace Aquila.Shell.Terminal
{
    public class TerminalWriter : TextWriter
    {
        private ITerminal _terminal;
        public TerminalWriter(ITerminal terminal)
        {
            _terminal = terminal;
        }
        public override Encoding Encoding => Encoding.ASCII;

        public override void Write(char value)
        {
            _terminal.Send(AnsiBuilder.Conv(value));
        }

        public override void Write(string value)
        {
            _terminal.Send(AnsiBuilder.Conv(value));
        }

        public override void WriteLine(string value)
        {
            _terminal.Send(AnsiBuilder.Conv(value));
            _terminal.Send(AnsiBuilder.Conv(C0.CR, C0.LF));   
        }
    }
}
