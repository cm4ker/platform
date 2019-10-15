using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using tterm.Ansi;
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

        private bool _isInitialized = false;

        private int _cursorX;
        private int _cursorY;
        private IConsole _c;

        private TerminalSize _size;

        private TerminalBufferChar[] _buffer;
        private List<TerminalBufferChar> _line;

        private List<string> _commandStory;
        private int _currentCommandStoryIndex = -1;

        private int _currentLineIndex = -1;

        public CommandApplication(ITerminal terminal)
        {
            _terminal = terminal;
            _c = (IConsole) terminal;
            _buffer = new TerminalBufferChar[_size.HeightRows * _size.WidthColumns];
            _line = new List<TerminalBufferChar>();
            _commandStory = new List<string>();
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

                    if (_currentLineIndex < _line.Count - 1)
                    {
                        _cursorX++;
                        SyncCursor();
                        _currentLineIndex++;
                    }

                    break;
                case TerminalCodeType.Text:
                    _cursorX += code.Text.Length;

                    if (_cursorX >= _size.WidthColumns)
                    {
                        _c.WriteLine();
                        _cursorX = 0;
                        SyncCursor();
                    }

                    if (code.Text.Length > 0)
                    {
                        var ch = new TerminalBufferChar(code.Text[0], code.CharAttributes);
                        if (_currentLineIndex == _line.Count - 1)
                            _line.Add(ch);
                        else
                        {
                            _line.RemoveAt(_currentLineIndex);
                            _line.Insert(_currentLineIndex, ch);
                        }

                        _currentLineIndex++;
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
                    _currentLineIndex = -1;

                    WriteLine();

                    var cmd = new string(_line.Select(x => x.Char).ToArray());
                    _commandStory.Add(cmd);
                    WriteLine($"The command is: {cmd}");
                    WriteProposal();

                    _line.Clear();
                    break;
                case TerminalCodeType.CursorBackward:
                    if (_currentLineIndex > _line.Count - 1)
                    {
                        _cursorX--;
                        SyncCursor();
                        _currentLineIndex--;
                    }

                    break;
                case TerminalCodeType.Backspace:
                    if (_currentLineIndex != -1)
                    {
                        _cursorX--;
                        SyncCursor();
                        _c.Write(" ");
                        SyncCursor();
                        _line.RemoveAt(_currentLineIndex);
                        _currentLineIndex--;
                    }

                    break;
                case TerminalCodeType.Delete:
                    SyncCursor();
                    break;
                case TerminalCodeType.CursorUp:
                    UpdateStoryCommand(true);
                    break;
                case TerminalCodeType.CursorDown:
                    UpdateStoryCommand(false);
                    break;
                case TerminalCodeType.DeviceStatusResponce:

                    _cursorX = code.Column;
                    _cursorY = code.Line;

                    if (!_isInitialized)
                    {
                        _isInitialized = true;
                        WriteProposal();
                    }

                    return;
            }
        }

        private void UpdateStoryCommand(bool isUp)
        {
            if (!_commandStory.Any())
                return;

            if (_currentCommandStoryIndex == -1)
            {
                _currentCommandStoryIndex = _commandStory.Count - 1;
            }
            else
            {
                if (isUp)
                {
                    if (_currentCommandStoryIndex > 0)
                        _currentCommandStoryIndex--;
                }
                else
                {
                    if (_currentCommandStoryIndex < _commandStory.Count)
                        _currentCommandStoryIndex++;
                }
            }

            if (_currentCommandStoryIndex < _commandStory.Count && _currentCommandStoryIndex >= 0)
            {
                var cmd = _commandStory[_currentCommandStoryIndex];

                _line = cmd.Select(x => new TerminalBufferChar(x, new CharAttributes())).ToList();
                _currentLineIndex = _line.Count - 1;
                ClearLine();
                WriteProposal();
                Write(cmd);
            }
            else
            {
                ClearLine();
                _currentLineIndex = -1;
                _line.Clear();
            }
        }

        private void WriteLine(string text = "")
        {
            _cursorY += 1;
            _cursorY = Math.Min((int) _size.HeightRows, _cursorY);
            _cursorX = 0;
            _c.WriteLine(text);
        }

        private void WriteProposal()
        {
            Write("> ");
        }

        private void ClearLine()
        {
            _cursorX = 0;
            SyncCursor();
            Write(new string(' ', (int) _size.WidthColumns));
            _cursorX = 0;
            SyncCursor();
        }

        private void Write(string text = "")
        {
            _c.Write(text);
            _cursorX += text.Length;
            //TODO: Если команда длиннее строки
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