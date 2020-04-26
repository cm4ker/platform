using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZenPlatform.Shell.Contracts.Ansi;

namespace ZenPlatform.Shell.Contracts
{
    public interface ITerminal
    {
        /// <summary>
        /// Потребить данные
        /// </summary>
        /// <param name="code">Терминальный код</param>
        void Consume(TerminalCode code);

        TerminalSize Size { get; set; }

        event EventHandler<byte[]> OnData;

        void Send(byte[] data);

        void Initialize(ITerminalApplication application);

        Task ConsoleOutputAsync(Stream stream);

        void LookInput();

        void UnLookInput();

        Stream GetInputStream();

    }
}
