using System;
using System.IO;
using System.Text;

namespace ZenPlatform.Shell.Terminal
{
    /// <summary>
    /// An implementation of <see cref="T:McMaster.Extensions.CommandLineUtils.IConsole" /> that does nothing.
    /// </summary>
    public class FakeConsole : McMaster.Extensions.CommandLineUtils.IConsole
    {
        public FakeConsole()
        {
            this.Error = this.Out = new StringWriter();
        }

        /// <summary>
        /// A shared instance of <see cref="T:McMaster.Extensions.CommandLineUtils.NullConsole" />.
        /// </summary>
        /// <summary>A writer that does nothing.</summary>
        public TextWriter Out { get; }

        /// <summary>A writer that does nothing.</summary>
        public TextWriter Error { get; }

        /// <summary>An empty reader.</summary>
        public TextReader In { get; } = (TextReader) new StringReader(string.Empty);

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