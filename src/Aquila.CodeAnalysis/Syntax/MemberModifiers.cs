using System;

namespace Aquila.CodeAnalysis.Syntax;

[Flags]
public enum MemberModifiers
{
    None = 0,
    Private = 0,
    Public = 0x1,
    Static = 0x2,
    Partial = 0x3
}