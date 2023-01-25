namespace Aquila.CodeAnalysis.Semantics;

internal class InModuleBinder : Binder
{
    public InModuleBinder(Binder next) : base(next)
    {
    }
}