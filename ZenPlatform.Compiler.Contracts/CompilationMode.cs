using System;

namespace ZenPlatform.Compiler.Contracts
{
    [Flags]
    public enum CompilationMode
    {
        Client = 1 << 1,
        Server = 1 << 2,
        
        /// <summary>
        /// Используется при объявлени объектов и там и там
        /// </summary>
        Shared = Client | Server
    }

    [Flags]
    public enum FunctionFlags
    {
        None = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        ClientServer = Client | Server,
        ServerClientCall = Client | Server | 1 << 2,
    }
}