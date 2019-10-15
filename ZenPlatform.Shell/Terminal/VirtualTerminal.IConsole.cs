using System;
using System.IO;
using ZenPlatform.Shell.Ansi;

namespace ZenPlatform.Shell.Terminal
{
    internal partial class VirtualTerminal : IConsole
    {
        public void WriteLine()
        {
            Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.LineFeed)));
        }

        public TextWriter In { get; }

        public TextWriter Out { get; }

        public TextWriter Error { get; }

        public bool CursorVisible { get; }

        public ConsoleColor BackgroundColor { get; set; }

        public ConsoleColor ForegroundColor { get; set; }

        public ConsoleKeyInfo ReadKey()
        {
            throw new NotImplementedException();
        }

        public void Beep()
        {
        }

        /// <summary>
        /// Индекс начиначется с 0
        /// </summary>
        public int CursorLeft => _cursorX;

        /// <summary>
        /// Индекс начиначется с 1
        /// </summary>
        public int CursorTop => _cursorY;

        public void WriteLine(string text)
        {
            Write(text);
            WriteLine();
        }

        public void Write(string text)
        {
            Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.Text, text)));
        }

        public void SetCursorPosition(int x, int y)
        {
            Send(AnsiBuilder.SetCursorPosCommand(y, x));
        }
    }
}