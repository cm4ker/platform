using System.Collections.Generic;
using Aquila.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis;


/// <summary>
/// Compilation state.
/// </summary>
internal sealed class CompilationState
{
    private readonly List<SourceMethodSymbolBase> _methodsToEmit = new();

    public IEnumerable<SourceMethodSymbolBase> MethodsToEmit => _methodsToEmit;

    public void RegisterMethodToEmit(SourceMethodSymbolBase method)
    {
        _methodsToEmit.Add(method);
    }
}