﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TextCopy;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.Shell.MiniTerm;
using ZenPlatform.Shell.Utility;
using ZenPlatform.SSH;
using ZenPlatform.SSH.Messages.Connection;
using Process = System.Diagnostics.Process;

namespace ZenPlatform.Shell.Terminal
{
    internal class TerminalSession : ITerminalSession
    {
        PipeStream _reader;
        AnonymousPipeServerStream _writer;
        private Process _cmdProcess;
        
        private readonly object _bufferSync = new object();
        private bool _disposed;


        public TerminalSession(TerminalSize size)
        {
            VTerminal = new VirtualTerminal(size);

            _writer = new AnonymousPipeServerStream(PipeDirection.Out);
            _reader = new AnonymousPipeClientStream(PipeDirection.In, _writer.GetClientHandleAsString());

            VTerminal.OnData += (s, a) => OnDataReceived(a);
        }


        public bool Connected { get; private set; }

        public Exception Exception { get; private set; }

        public VirtualTerminal VTerminal { get; }

        public event EventHandler<uint> CloseReceived;
        public event EventHandler<byte[]> DataReceived;


        public void Close()
        {
            _writer.WriteByte(0x03);
        }

        public void ConsumeData(byte[] data)
        {
            _writer.Write(data, 0, data.Length);
        }

        public void ChangeSize(TerminalSize size)
        {
            VTerminal.SetSize(size);
        }

        public void Run()
        {
            VTerminal.Open(new CommandApplication(VTerminal));
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


        private void SessionErrored(Exception ex)
        {
            Connected = false;
            Exception = ex;
        }

        private void SessionClosed()
        {
            Connected = false;
        }
    }



}