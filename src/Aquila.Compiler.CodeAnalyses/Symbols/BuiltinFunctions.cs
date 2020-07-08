using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aquila.Language.Ast.Symbols
{
    internal static class BuiltinFunctions
    {
        // public static readonly MethodSymbol Print = new MethodSymbol("print",
        //     ImmutableArray.Create(new ParameterSymbol("text", NamedTypeSymbol.Any, 0)), NamedTypeSymbol.Void);
        //
        // public static readonly MethodSymbol Input =
        //     new MethodSymbol("input", ImmutableArray<ParameterSymbol>.Empty, NamedTypeSymbol.String);
        //
        // public static readonly MethodSymbol Rnd = new MethodSymbol("rnd",
        //     ImmutableArray.Create(new ParameterSymbol("max", NamedTypeSymbol.Int, 0)), NamedTypeSymbol.Int);

        internal static IEnumerable<MethodSymbol> GetAll()
            => typeof(BuiltinFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(MethodSymbol))
                .Select(f => (MethodSymbol) f.GetValue(null)!);
    }
}