using System;

namespace ZenPlatform.Compiler
{
    [Flags]
    public enum CompilationMode
    {
        Client = 1 << 0 | 1 << 2,
        Server = 1 << 1,
    }

    [Flags]
    public enum FunctionFlags
    {
        None = 0,
        Client = 1 << 0,
        Server = 1 << 1,
        ServerClientCall = Client | 1 << 2,
        ClientServer = Client | Server
    }
}