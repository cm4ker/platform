using System;
using System.IO;

namespace Aquila.Compiler.Roslyn
{
    public class Pair : IDisposable
    {
        private readonly TextWriter _tw;
        private readonly string _close;
        private readonly bool _newLine;

        public Pair(TextWriter tw, string open, string close, bool newLine = false)
        {
            _tw = tw;
            _close = close;
            _newLine = newLine;

            if (newLine)
                tw.W("\n");
            _tw.W(open);
            if (_newLine)
                _tw.W("\n");
        }

        public void Dispose()
        {
            if (_newLine)
                _tw.W("\n");
            _tw.W(_close);
        }
    }
}