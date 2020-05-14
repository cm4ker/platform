using System;

namespace Aquila.Core.Contracts
{
    [Flags]
    public enum CompilationMode
    {
        Client = 1 << 0,
        Server = 1 << 1,

        /// <summary>
        /// Используется при объявлени объектов и там и там
        /// </summary>
        Shared = Client | Server
    }

    [Flags]
    public enum FunctionFlags
    {
        //CompilationMode и FunctionFlags должны совпадать

        None = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        ClientServer = Client | Server,
        ServerClientCall = Client | Server | 1 << 2,
        IsOperation = 1 << 3
    }
}