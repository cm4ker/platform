using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    /// <summary>
    /// Обычное коммандное приложение с коммандной строкой
    /// </summary>
    internal class CommandApplication : ITerminalApplication
    {
        private readonly ITerminal _terminal;

        private int _cursorX;
        private int _cursorY;
        private IConsole _c;

        private TerminalSize _size;

        private List<char> _line;

        public CommandApplication(ITerminal terminal)
        {
            _terminal = terminal;
            _c = (IConsole) terminal;

            _line = new List<char>();
        }

        public void Open()
        {
            _c.CursorPositionRequest();
        }

        public void Close()
        {
        }

        public void Consume(TerminalCode code)
        {
            switch (code.Type)
            {
                case TerminalCodeType.CursorForward:
                    _cursorX++;
                    SyncCursor();
                    break;
                case TerminalCodeType.Text:
                    _cursorX += code.Text.Length;

                    _c.Write(code.Text);
                    SyncCursor();
                    break;
                case TerminalCodeType.CursorBackward:
                    _cursorX--;
                    SyncCursor();
                    break;
                case TerminalCodeType.Backspace:
                    _cursorX--;
                    SyncCursor();
                    _c.Write(" ");
                    SyncCursor();
                    break;
                case TerminalCodeType.Delete:
                    SyncCursor();
                    break;
                case TerminalCodeType.CursorUp:
                    _cursorY--;
                    SyncCursor();
                    break;
                case TerminalCodeType.CursorDown:
                    _cursorY++;
                    SyncCursor();
                    break;
                case TerminalCodeType.DeviceStatusResponce:
                    _cursorX = code.Column;
                    _cursorY = code.Line;
                    return;
            }
        }

        private void SyncCursor()
        {
            _c.SetCursorPosition(_cursorX + 1, _cursorY + 1);
        }
    }
}