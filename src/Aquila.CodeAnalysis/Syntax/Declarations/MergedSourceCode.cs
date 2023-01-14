using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Syntax;
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
        private List<HtmlDecl> _views;

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

        public IEnumerable<HtmlDecl> GetViews()
        {
            EnsureViews();
            return _views;
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

            foreach (var unit in _trees.Where(x=>!x.IsView).Select(x => x.GetCompilationUnitRoot()))
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

        private void EnsureViews()
        {
            if (_views != null)
                return;

            _views = new List<HtmlDecl>();
            foreach (var unit in _trees.Where(x=>x.IsView).Select(x=>x.GetCompilationUnitRoot()))
            {
                if(unit != null && unit.Html != null)
                    _views.Add(unit.Html);
            }
        }
    }
}