using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TextCopy;
using ZenPlatform.Core.Logging;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.Shell.Contracts.Ansi;
using ZenPlatform.Shell.Contracts;
using ZenPlatform.Shell.Utility;
using ZenPlatform.SSH;
using ZenPlatform.SSH.Messages.Connection;
using Process = System.Diagnostics.Process;

namespace ZenPlatform.Shell.Terminal
{
    public class TerminalSession : ITerminalSession
    {
        PipeStream _reader;
        AnonymousPipeServerStream _writer;

        private readonly object _bufferSync = new object();
        private bool _disposed;
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        public TerminalSession(ITerminal terminal, IServiceProvider serviceProvider, ILogger<TerminalSession> logger)
        {
            VTerminal = terminal;
            VTerminal.OnData += (s, a) => OnDataReceived(a);

            _serviceProvider = serviceProvider;
            _logger = logger;

            _writer = new AnonymousPipeServerStream(PipeDirection.Out);
            _reader = new AnonymousPipeClientStream(PipeDirection.In, _writer.GetClientHandleAsString());
            
        }


        public bool Connected { get; private set; }

        public Exception Exception { get; private set; }

        public ITerminal VTerminal { get; private set; }

        public event EventHandler<uint> CloseReceived;
        public event EventHandler<byte[]> DataReceived;


        public void Close()
        {
            
            SessionClosed();
        }

        public void ConsumeData(byte[] data)
        {
            _writer.Write(data, 0, data.Length);
        }

        public void ChangeSize(TerminalSize size)
        {
            VTerminal.Size = size;
        }

        public void Run(ITerminalApplication application)
        {
            
            VTerminal?.Initialize(application);
            application.Open(VTerminal);
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
            await ConsoleOutputAsync(_reader);
            
//            try
//            {
//             
//            }
//            catch (Exception ex)
//            {
//                SessionErrored(ex);
//                return;
//            }

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
                    VTerminal.Consume(code);
                }
            }
        }

        private void OnDataReceived(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            OnDataReceived(data);
        }

        private void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        private void OnCloseReceived(uint e)
        {
            CloseReceived?.Invoke(this, e);
        }


        private void SessionErrored(Exception ex)
        {
            Connected = false;
            Exception = ex;
            OnCloseReceived(1);
        }

        private void SessionClosed()
        {
            Connected = false;
            OnCloseReceived(0x00);
        }
    }



}