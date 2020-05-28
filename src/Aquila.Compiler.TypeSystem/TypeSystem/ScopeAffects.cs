using System;

namespace Aquila.Compiler.Aqua.TypeSystem
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