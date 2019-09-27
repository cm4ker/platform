using System;
using System.Collections.Generic;
using tterm.Ansi;
using ZenPlatform.Shell.Ansi;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    /// <summary>
    /// Терминальная сессия - мостик между сессией и терминалом
    /// </summary>
    internal interface ITerminalSession : IDisposable
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

        void Run();
    }

    /// <summary>
    /// Непосредственно терминал
    /// </summary>
    internal interface ITerminal : IHostAppManager
    {
        /// <summary>
        /// Текущая колонка
        /// </summary>
        int CursorX { get; }

        /// <summary>
        /// Текущая 
        /// </summary>
        int CursorY { get; }

        /// <summary>
        /// Потребить данные
        /// </summary>
        /// <param name="code">Терминальный код</param>
        void Consume(TerminalCode code);
    }

    internal interface IHostAppManager
    {
        /// <summary>
        /// Стэк запущенных приложений
        /// </summary>
        ExtendedStack<ITerminalApplication> Apps { get; }

        /// <summary>
        /// Текущее активное приложение
        /// </summary>
        ITerminalApplication CurrentActive { get; }

        /// <summary>
        /// Закрыть приложение
        /// </summary>
        /// <param name="app"></param>
        void Close(ITerminalApplication app);

        /// <summary>
        /// Открыть приложение
        /// </summary>
        /// <param name="app"></param>
        void Open(ITerminalApplication app);
    }

    public enum TerminalApplicationState
    {
        /// <summary>
        ///  Не активное приложение
        /// </summary>
        NotActive,

        /// <summary>
        ///  Текущее рабочее приложение
        /// </summary>
        Run,
    }

    internal interface ITerminalApplication
    {
        TerminalApplicationState State { get; }

        event EventHandler<object> Data;

        void Consume(TerminalCode code);

        void SetState(TerminalApplicationState newState);
    }
}