using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;

namespace Aquila.Syntax.Declarations;

public class MergeModuleDecl
{
    private readonly CompilationUnitSyntax _firstElement;
    private readonly ImmutableArray<CompilationUnitSyntax> _units;
    private ImmutableArray<MergedTypeDecl> _types;
    private ImmutableArray<FuncDecl> _func;

    public MergeModuleDecl(ImmutableArray<CompilationUnitSyntax> units)
    {
        _firstElement = units.First();
        _units = units;
    }

    ImmutableArray<MergedTypeDecl> EnsureTypesCore()
    {
        var builder = ImmutableArray.CreateBuilder<MergedTypeDecl>();

        if (_types == null || _types.IsDefault)
        {
            foreach (var unit in _units)
            {
                foreach (var type in unit.Types)
                {
                    //skip partial types (types generated from metadata)
                    //TODO: rename partial types to metadata types / synthesized ???
                    if (AstUtils.GetModifiers(type.Modifiers).IsPartial())
                    {
                        continue;
                    }

                    var typeName = type.Name.GetUnqualifiedName().Identifier.Text;

                    var funcs = unit.Functions.Where(x => x.FuncOwner?.OwnerType.GetName() == typeName)
                        .ToImmutableArray();

                    builder.Add(new MergedTypeDecl(type, funcs));
                }
            }

            _types = builder.ToImmutableArray();
        }

        return _types;
    }

    ImmutableArray<FuncDecl> EnsureFunctionsCore()
    {
        if (_func == null || _func.IsDefaultOrEmpty)
            _func = _units.SelectMany(x => x.Functions).ToImmutableArray();

        return _func;
    }

    //TODO: make main is constant value as default module name
    public string Name => _firstElement.ModuleName;

    public IEnumerable<FuncDecl> ModuleFunctions => EnsureFunctionsCore().Where(x => x.FuncOwner == null);

    public IEnumerable<FuncDecl> OwnedFunctions => EnsureFunctionsCore().Where(x => x.FuncOwner != null);

    public IEnumerable<MergedTypeDecl> Types => EnsureTypesCore();
}