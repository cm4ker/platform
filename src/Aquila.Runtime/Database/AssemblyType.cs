using System;

namespace Aquila.Core.Assemlies
{
    [Flags]
    public enum FileType
    {
        Unknown = 0,
        Assembly = 1 << 0,
        MainAssembly = 1 << 1,
    }
}