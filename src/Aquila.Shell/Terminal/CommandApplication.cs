using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Aquila.Cli;
using Aquila.Shell.Ansi;
using Aquila.Shell.Contracts;
using Aquila.Shell.Contracts.Ansi;
using Aquila.Shell.Utility;
using Aquila.SSH;


namespace Aquila.Shell.Terminal
{
    /// <summary>
    /// Обычное коммандное приложение с коммандной строкой
    /// </summary>
    public class CommandApplication : ITerminalApplication
    {
        private ITerminal _terminal;

        private bool _isInitialized = false;

        private int _cursorX;
        private int _cursorY;
       
        

        private List<TerminalBufferChar> _line;

        private List<string> _commandStory;
        private int _currentCommandStoryIndex = -1;

        private int _currentLineIndex = -1;
        private IServiceProvider _serviceProvider;
        public CommandApplication(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _line = new List<TerminalBufferChar>();
            _commandStory = new List<string>();
        }

        public void Open(ITerminal terminal)
        {
            _terminal = terminal;

            WriteLine(@"
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

            WriteLine();
            _terminal.Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.DeviceStatusRequest)));
        }

        public void Close()
        {
        }

        private void WriteLine()
        {
            _cursorY += 1;
            _cursorY = Math.Min((int)_terminal.Size.HeightRows, _cursorY);
            _cursorX = 0;
            _terminal.Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.LineFeed)));
        }
        private void WriteLine(string text)
        {
            Write(text);
            WriteLine();
        }

        private void Write(string text)
        {
            _terminal.Send(AnsiBuilder.Build(new TerminalCode(TerminalCodeType.Text, text)));
            _cursorX += text.Length;
        }

        private void SetCursorPosition(int x, int y)
        {
            _terminal.Send(AnsiBuilder.SetCursorPosCommand(y, x));
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
                    //_cursorX += code.Text.Length;

                    if (_cursorX >= _terminal.Size.WidthColumns)
                    {
                        WriteLine();
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

                    Write(code.Text);
                    SyncCursor();
                    break;
                case TerminalCodeType.LineFeed:
                    //_cursorY += 1;
                    //_cursorX = 0;
                    WriteLine();
                    break;

                case TerminalCodeType.CarriageReturn:
                    _currentLineIndex = -1;

                    WriteLine();

                    var cmd = new string(_line.Select(x => x.Char).ToArray());
                    _commandStory.Add(cmd);
                    //WriteLine($"The command is: {cmd}");

                    RunCommand(cmd);

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
                        Write(" ");
                        _cursorX--;
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


        private void WriteProposal()
        {
            Write("> ");
        }

        private void ClearLine()
        {
            _cursorX = 0;
            SyncCursor();
            Write(new string(' ', (int)_terminal.Size.WidthColumns));
            _cursorX = 0;
            SyncCursor();
        }

  


        private void SyncCursor()
        {
            SetCursorPosition(_cursorX + 1, _cursorY + 1);
        }


        private void RunCommand(string cmd)
        {
            var args = cmd.Split(' ');

            //var fakeConsole = _serviceProvider.GetRequiredService<McMaster.Extensions.CommandLineUtils.IConsole>();

            //var app = CliBuilder.Build(fakeConsole);
            var app = _serviceProvider.GetRequiredService<ICommandLineInterface>();

            _terminal.LookInput();
            var result = app.Execute(args);
            _terminal.UnLookInput();
            // fakeConsole.Out.Flush();
            /// Write(fakeConsole.Out.ToString());

        }
    }
}