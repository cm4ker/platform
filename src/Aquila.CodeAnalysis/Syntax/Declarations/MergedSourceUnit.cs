using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;

namespace Aquila.Syntax.Declarations
{
    /* Project > Files > Ast > SourceDeclarationProvider > SemanticProvider */

    /// <summary>
    /// Response for merging multiply source units into one
    /// </summary>
    public class MergedSourceUnit
    {
        private List<CompilationUnitSyntax> _units;

        public MergedSourceUnit(IEnumerable<CompilationUnitSyntax> units)
        {
            _units = units.ToList();
        }

        public IEnumerable<CompilationUnitSyntax> Units => _units;

        public IEnumerable<MergedComponentDecl> GetComponents()
        {
            Dictionary<string, List<ComponentDecl>> merged = new Dictionary<string, List<ComponentDecl>>();

            foreach (var unit in _units)
            {
                foreach (var com in unit.Components)
                {
                    if (merged.TryGetValue(com.Name.GetText().ToString(), out var list))
                    {
                        list.Add(com);
                    }
                    else
                        merged.Add(com.Name.GetUnqualifiedName().Identifier.Text, new List<ComponentDecl> { com });
                }
            }

            return merged.Select(x => new MergedComponentDecl(x.Value.ToImmutableArray()));
        }

        public void Add(CompilationUnitSyntax unit)
        {
            if (!_units.Contains(unit))
                _units.Add(unit);
        }

        public void AddRange(IEnumerable<CompilationUnitSyntax> units)
        {
            units.Foreach(Add);
        }
    }
}