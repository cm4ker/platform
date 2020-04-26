using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using ZenPlatform.Shell.Contracts;

namespace ZenPlatform.Shell.Terminal
{
    public class TerminalConsole : McMaster.Extensions.CommandLineUtils.IConsole
    {
        private ITerminal _terminal;
        public TerminalConsole(ITerminal terminal)
        {
            _terminal = terminal;
            this.Error = this.Out = new TerminalWriter(_terminal);
            this.In = new StreamReader(_terminal.GetInputStream());// (TextReader)new StringReader(string.Empty);
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
