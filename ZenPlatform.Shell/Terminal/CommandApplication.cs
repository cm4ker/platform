using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using tterm.Terminal;
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

        private TerminalBufferChar[] _buffer;

        public CommandApplication(ITerminal terminal)
        {
            _terminal = terminal;
            _c = (IConsole) terminal;
            _buffer = new TerminalBufferChar[_size.HeightRows * _size.WidthColumns];
        }

        public void Open(TerminalSize size)
        {
            _size = size;

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

                    if (_cursorX >= _size.WidthColumns)
                    {
                        _c.WriteLine();
                        _cursorX = 0;
                        SyncCursor();
                    }

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
//            if (size != _size)
//            {
//                var srcSize = _size;
//                var dstSize = size;
//
//                var srcBuffer = _buffer;
//                var dstBuffer = new TerminalBufferChar[dstSize.HeightRows * dstSize.WidthColumns];
//
//                int srcLeft = 0;
//                int srcRight = Math.Min((int) srcSize.WidthColumns, (int) dstSize.WidthColumns) - 1;
//                int srcTop = Math.Max(0, _cursorY - (int) dstSize.HeightRows + 1);
//                int srcBottom = srcTop + Math.Min((int) srcSize.HeightRows, (int) dstSize.HeightRows) - 1;
//                int dstLeft = 0;
//                int dstTop = 0;
//
//                srcBuffer.CopyBufferToBuffer(srcSize, srcLeft, srcTop, srcRight, srcBottom,
//                    dstBuffer, dstSize, dstLeft, dstTop);
//
//                _buffer = dstBuffer;
//                _size = dstSize;
//
//                _cursorY = Math.Min(_cursorY, (int) _size.HeightRows - 1);
//                _cursorX = Math.Min(_cursorX, (int) _size.WidthColumns - 1);
//                
//                SyncCursor();
//            }
        }

        private void SyncCursor()
        {
            _c.SetCursorPosition(_cursorX + 1, _cursorY + 1);
        }
    }
}