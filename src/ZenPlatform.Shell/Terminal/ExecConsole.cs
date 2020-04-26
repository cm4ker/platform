using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using ZenPlatform.Shell.Ansi;

namespace ZenPlatform.Shell.Terminal
{


    

    public class ExecConsole : McMaster.Extensions.CommandLineUtils.IConsole
    {
        public class EventTextWriter : TextWriter
        {
            public override Encoding Encoding => Encoding.Default;
            public event EventHandler<byte[]> DataReceived;

            public override void Write(char value)
            {
                DataReceived?.Invoke(this, AnsiBuilder.Conv(value));
            }

            public override void Write(string value)
            {
                DataReceived?.Invoke(this, AnsiBuilder.Conv(value));
            }

            public override void WriteLine(string value)
            {
                DataReceived?.Invoke(this, AnsiBuilder.Conv(value));
                DataReceived?.Invoke(this, AnsiBuilder.Conv(C0.CR, C0.LF));
            }
        }

        public event EventHandler<byte[]> DataReceived;

        public ExecConsole()
        {
            var _out = new EventTextWriter();
            this.Error = this.Out = _out;
            _out.DataReceived += (e, c) => DataReceived.Invoke(e, c);

            _writer = new AnonymousPipeServerStream(PipeDirection.Out);
            var _reader = new AnonymousPipeClientStream(PipeDirection.In, _writer.GetClientHandleAsString());
            this.In = new StreamReader(_reader);
        }

        private AnonymousPipeServerStream _writer;

        public void ConsumeData(byte[] data)
        {
            _writer.Write(data, 0, data.Length);
           
        }

        public void Close()
        {
            _writer.Flush();
            _writer.Close();
        }
        /// <summary>
        /// A shared instance of <see cref="T:McMaster.Extensions.CommandLineUtils.NullConsole" />.
        /// </summary>
        /// <summary>A writer that does nothing.</summary>
        public TextWriter Out { get; }

        /// <summary>A writer that does nothing.</summary>
        public TextWriter Error { get; }

        /// <summary>An empty reader.</summary>
        public TextReader In { get; }

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public bool IsInputRedirected
        {
            get { return false; }
        }

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public bool IsOutputRedirected
        {
            get { return false; }
        }

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public bool IsErrorRedirected
        {
            get { return false; }
        }

        /// <inheritdoc />
        public ConsoleColor ForegroundColor { get; set; }

        /// <inheritdoc />
        public ConsoleColor BackgroundColor { get; set; }

        /// <summary>This event never fires.</summary>
        public event ConsoleCancelEventHandler CancelKeyPress
        {
            add { }
            remove { }
        }

        /// <summary>Does nothing.</summary>
        public void ResetColor()
        {
        }
    }
}
