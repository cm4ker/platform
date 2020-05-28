using System;

namespace Aquila.Core.Contracts.TypeSystem
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