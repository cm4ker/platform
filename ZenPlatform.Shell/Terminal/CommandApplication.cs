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
            _c.WriteLine(@"
 _____               ___ _       _    __                      
/ _  / ___ _ __     / _ \ | __ _| |_ / _| ___  _ __ _ __ ___  
\// / / _ \ '_ \   / /_)/ |/ _` | __| |_ / _ \| '__| '_ ` _ \ 
 / //\  __/ | | | / ___/| | (_| | |_|  _| (_) | |  | | | | | |
/____/\___|_| |_| \/    |_|\__,_|\__|_|  \___/|_|  |_| |_| |_|
                                                              
 _____                    _             _                     
/__   \___ _ __ _ __ ___ (_)_ __   __ _| |                    
  / /\/ _ \ '__| '_ ` _ \| | '_ \ / _` | |                    
 / / |  __/ |  | | | | | | | | | | (_| | |                    
 \/   \___|_|  |_| |_| |_|_|_| |_|\__,_|_|                    ");

            _c.WriteLine();
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
                case TerminalCodeType.LineFeed:
                    _cursorY += 1;
                    _cursorX = 0;
                    _c.WriteLine();
                    break;

                case TerminalCodeType.CarriageReturn:
                    _cursorY += 1;
                    _cursorY = Math.Min((int) _size.HeightRows, _cursorY);
                    _cursorX = 0;
                    _c.WriteLine();
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

        private void RedrawCurrentLine()
        {
        }

        public void SetSize(TerminalSize size)
        {
            _size = size;

            if (_cursorX >= _size.WidthColumns)
            {
                _cursorX -= (int) _size.WidthColumns;
                _cursorY += 1;

                if (_cursorY > _size.HeightRows)
                {
                    _c.WriteLine();
                    _cursorY = (int) _size.HeightRows;
                }
            }
        }

        private void SyncCursor()
        {
            _c.SetCursorPosition(_cursorX + 1, _cursorY + 1);
        }
    }
}