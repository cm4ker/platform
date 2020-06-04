using System;

namespace Aquila.Compiler.Aqua
{
    [Flags]
    public enum ScopeAffects
    {
        Unknown,
        Query,
        UX,
        Code
    }
}