using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        private List<SourceUnit> _units;

        public MergedSourceUnit(IEnumerable<SourceUnit> units)
        {
            _units = units.ToList();
        }

        public IEnumerable<SourceUnit> Units => _units;

        public IEnumerable<MergedComponentDecl> GetComponents()
        {
            Dictionary<string, List<ComponentDecl>> merged = new Dictionary<string, List<ComponentDecl>>();

            foreach (var unit in _units)
            {
                foreach (var ns in unit.Components)
                {
                    if (merged.TryGetValue(ns.Identifier.Text, out var list))
                    {
                        list.Add(ns);
                    }
                    else
                        merged.Add(ns.Identifier.Text, new List<ComponentDecl> {ns});
                }
            }

            return merged.Select(x => new MergedComponentDecl(x.Value.ToImmutableArray()));
        }

        public void Add(SourceUnit unit)
        {
            if (!_units.Contains(unit))
                _units.Add(unit);
        }

        public void AddRange(IEnumerable<SourceUnit> units)
        {
            units.Foreach(Add);
        }
    }
}