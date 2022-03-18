using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.Syntax.Declarations
{
    /* Project > Files > Ast > SourceDeclarationProvider > SemanticProvider */

    /// <summary>
    /// Response for merging multiply source units into one
    /// </summary>
    public class MergedSourceCode
    {
        /// <summary>
        /// Collection version, increased when a syntax tree is added or removed.
        /// </summary>
        public int Version => _version;

        int _version = 0;


        readonly ConcurrentDictionary<SyntaxTree, int> _ordinalMap = new ConcurrentDictionary<SyntaxTree, int>();
        private readonly ConcurrentBag<AquilaSyntaxTree> _trees = new ConcurrentBag<AquilaSyntaxTree>();

        private List<MergeModuleDecl> _mergedModules;

        public MergedSourceCode()
        {
        }

        public IDictionary<SyntaxTree, int> OrdinalMap => _ordinalMap;

        public ImmutableArray<AquilaSyntaxTree> SyntaxTrees => _trees.ToImmutableArray();

        public void AddSyntaxTreeRange(IEnumerable<AquilaSyntaxTree> trees)
        {
            trees.ForEach(AddSyntaxTreeCore);
        }

        public void AddSyntaxTree(AquilaSyntaxTree tree)
        {
            AddSyntaxTreeCore(tree);
        }

        private void AddSyntaxTreeCore(AquilaSyntaxTree tree)
        {
            Contract.ThrowIfNull(tree);

            _ordinalMap.Add(tree, _ordinalMap.Count);
            _trees.Add(tree);

            _version++;
        }

        public IEnumerable<MergeModuleDecl> GetModules()
        {
            EnsureModules();
            return _mergedModules;
        }

        public MergeModuleDecl GetModule(string name)
        {
            return GetModules().FirstOrDefault(x => x.Name == name);
        }

        private void EnsureModules()
        {
            if (_mergedModules != null)
                return;

            var modDic = new Dictionary<string, List<CompilationUnitSyntax>>();

            foreach (var unit in _trees.Select(x => x.GetCompilationUnitRoot()))
            {
                if (unit != null)
                    if (modDic.TryGetValue(unit.ModuleName, out var list))
                    {
                        list.Add(unit);
                    }
                    else
                        modDic.Add(unit.ModuleName, new List<CompilationUnitSyntax> { unit });
            }

            var modules = modDic.Select(x => new MergeModuleDecl(x.Value.ToImmutableArray())).ToList();

            Interlocked.CompareExchange(ref _mergedModules, modules, null);
        }
    }


    public class MergedTypeDecl
    {
        public MergedTypeDecl(TypeDecl type, IEnumerable<FuncDecl> funcs)
        {
            TypeDecl = type;
            FuncDecls = funcs;
        }

        public string Name => TypeDecl.Name.GetUnqualifiedName().Identifier.Text;

        public TypeDecl TypeDecl { get; }
        public IEnumerable<FuncDecl> FuncDecls { get; }
    }

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

        ImmutableArray<FuncDecl> EnsureFinctionsCore()
        {
            if (_func == null || _func.IsDefaultOrEmpty)
                _func = _units.SelectMany(x => x.Functions).ToImmutableArray();

            return _func;
        }

        //TODO: make main is constant value as default module name
        public string Name => _firstElement.ModuleName;

        public IEnumerable<FuncDecl> ModuleFunctions => EnsureFinctionsCore().Where(x => x.FuncOwner == null);

        public IEnumerable<FuncDecl> OwnedFunctions => EnsureFinctionsCore().Where(x => x.FuncOwner != null);

        public IEnumerable<MergedTypeDecl> Types => EnsureTypesCore();
    }
}