using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FxSsh.Messages.Connection;
using MiniTerm;
using TextCopy;
using tterm.Ansi;
using tterm.Utility;

namespace tterm.Terminal
{
    internal class TerminalSession : ITerminal
    {
        private readonly StreamWriter _ptyWriter;

        PipeStream reader;
        AnonymousPipeServerStream writer;


        private readonly object _bufferSync = new object();
        private bool _disposed;

        public event EventHandler TitleChanged;
        public event EventHandler OutputReceived;
        public event EventHandler BufferSizeChanged;
        public event EventHandler Finished;

        public string Title { get; set; }
        public bool Active { get; set; }
        public bool Connected { get; private set; }
        public bool ErrorOccured { get; private set; }
        public Exception Exception { get; private set; }

        public TerminalBuffer Buffer { get; }

        public TerminalSize Size
        {
            get => Buffer.Size;
            set
            {
                if (Buffer.Size != value)
                {
                    Buffer.Size = value;
                    BufferSizeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TerminalSession(TerminalSize size)
        {
            Buffer = new TerminalBuffer(size);

            writer = new AnonymousPipeServerStream(PipeDirection.Out);
            reader = new AnonymousPipeClientStream(PipeDirection.In, (writer.GetClientHandleAsString()));

            _ptyWriter = new StreamWriter(writer, Encoding.UTF8)
            {
                AutoFlush = true
            };
            RunOutputLoop();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        private async void RunOutputLoop()
        {
            try
            {
                await ConsoleOutputAsync(reader);
            }
            catch (Exception ex)
            {
                SessionErrored(ex);
                return;
            }

            SessionClosed();
        }

        private Task ConsoleOutputAsync(Stream stream)
        {
            return Task.Run(async delegate
            {
                var ansiParser = new AnsiParser();
                var sr = new StreamReader(stream);
                do
                {
                    int offset = 0;
                    var buffer = new char[1024];
                    int readChars = await sr.ReadAsync(buffer, offset, buffer.Length - offset);
                    if (readChars > 0)
                    {
                        var reader = new ArrayReader<char>(buffer, 0, readChars);
                        var codes = ansiParser.Parse(reader);
                        ReceiveOutput(codes);
                    }
                } while (!sr.EndOfStream);
            });
        }

        private void ReceiveOutput(IEnumerable<TerminalCode> codes)
        {
            lock (_bufferSync)
            {
                foreach (var code in codes)
                {
                    ProcessTerminalCode(code);
                }
            }
        }

        private void ProcessTerminalCode(TerminalCode code)
        {
            switch (code.Type)
            {
                case TerminalCodeType.ResetMode:
                    Buffer.ShowCursor = false;
                    break;
                case TerminalCodeType.SetMode:
                    Buffer.ShowCursor = true;

                    // HACK We want clear to reset the window position but not general typing.
                    //      We therefore reset the window only if the cursor is moved to the top.
                    if (Buffer.CursorY == 0)
                    {
                        Buffer.WindowTop = 0;
                    }

                    break;
                case TerminalCodeType.Text:
                    
                    Buffer.Type(code.Text);

                    var data = Encoding.UTF8.GetBytes(Buffer.GetText());
                    
                    
                    DataReceived?.Invoke(this, );
                    break;
                case TerminalCodeType.LineFeed:
                    //new line feed
                    break;
                case TerminalCodeType.CarriageReturn:
                    //Flush command to the handler
                    break;
                case TerminalCodeType.CharAttributes:
                    Buffer.CurrentCharAttributes = code.CharAttributes;
                    break;
                case TerminalCodeType.CursorPosition:
                    Buffer.CursorX = code.Column;
                    Buffer.CursorY = code.Line;
                    break;
                case TerminalCodeType.CursorUp:
                    Buffer.CursorY -= code.Line;
                    break;
                case TerminalCodeType.CursorCharAbsolute:
                    Buffer.CursorX = code.Column;
                    break;
                case TerminalCodeType.EraseInLine:
                    if (code.Line == 0)
                    {
                        Buffer.ClearBlock(Buffer.CursorX, Buffer.CursorY, Buffer.Size.Columns - 1, Buffer.CursorY);
                    }

                    break;
                case TerminalCodeType.EraseInDisplay:
                    Buffer.Clear();
                    Buffer.CursorX = 0;
                    Buffer.CursorY = 0;
                    break;
                case TerminalCodeType.SetTitle:
                    Title = code.Text;
                    TitleChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public void Write(string text)
        {
            _ptyWriter.Write(text);
        }

        public void Paste()
        {
            string text = Clipboard.GetText();
            if (!String.IsNullOrEmpty(text))
            {
                Write(text);
            }
        }

        private void SessionErrored(Exception ex)
        {
            Connected = false;
            Exception = ex;
            Finished?.Invoke(this, EventArgs.Empty);
        }

        private void SessionClosed()
        {
            Connected = false;
            Finished?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<uint> CloseReceived;
        public event EventHandler<byte[]> DataReceived;

        public void OnClose()
        {
            writer.WriteByte(0x03);
        }

        public void OnInput(byte[] data)
        {
            writer.Write(data, 0, data.Length);
        }

        public void OnSizeChanged(ConsoleSize size)
        {
        }

        public void Run()
        {
            RunOutputLoop();
        }
    }
}