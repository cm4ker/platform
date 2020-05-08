using System;

namespace Aquila.Shell.Contracts
{
    public interface ITerminalSession : IDisposable
    {
        /// <summary>
        /// При закрытии
        /// </summary>
        event EventHandler<uint> CloseReceived;

        /// <summary>
        /// При получении данных
        /// </summary>
        event EventHandler<byte[]> DataReceived;

        /// <summary>
        ///  Закрыть
        /// </summary>
        void Close();

        /// <summary>
        /// Потребить данные
        /// </summary>
        /// <param name="data">Данные</param>
        void ConsumeData(byte[] data);

        /// <summary>
        /// Изменить размер терминала
        /// </summary>
        /// <param name="size">Размер</param>
        void ChangeSize(TerminalSize size);

        void Run(ITerminalApplication application);
    }
}
