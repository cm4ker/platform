using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Syntax.Ast;

namespace Aquila.Syntax.Declarations
{
    /// <summary>
    /// Response for merging many namespace declaration into the one 
    /// </summary>
    public class MergedComponentDecl
    {
        private readonly ImmutableArray<ComponentDecl> _declareations;
        private ComponentDecl _firstElem;

        public MergedComponentDecl(ImmutableArray<ComponentDecl> declarations)
        {
            if (!declarations.Any()) throw new Exception("The declarations can't be empty");

            _declareations = declarations;
            _firstElem = this._declareations.First();
        }

        public QualifiedIdentifierToken Identifier
        {
            get => _firstElem.Identifier;
        }

        private IEnumerable<ExtendDecl> Extends
        {
            get => _declareations.SelectMany(x => x.Extends);
        }

        public ImmutableArray<MergedExtendDecl> GetExtends()
        {
            var merged = new Dictionary<string, List<ExtendDecl>>();

            foreach (var extendDecl in Extends)
            {
                if (merged.TryGetValue($"{Identifier.Text}.{extendDecl.Identifier.Text}", out var list))
                {
                    list.Add(extendDecl);
                }
                else
                    merged.Add($"{Identifier.Text}.{extendDecl.Identifier.Text}",
                        new List<ExtendDecl> {extendDecl});
            }

            return merged.Select(x => new MergedExtendDecl(x.Value.ToImmutableArray())).ToImmutableArray();
        }
    }
}