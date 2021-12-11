using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.CodeAnalysis.Syntax;

namespace Aquila.Syntax.Declarations
{
    /// <summary>
    /// Response for merging many extend declaration into the one 
    /// </summary>
    public class MergedExtendDecl
    {
        private readonly ImmutableArray<ExtendDecl> extendsDecls;
        private ExtendDecl first;

        public MergedExtendDecl(ImmutableArray<ExtendDecl> extendsDecls)
        {
            this.extendsDecls = extendsDecls;
            first = extendsDecls.First();
        }

        public NameEx Identifier => first.Name;

        public IEnumerable<MethodDecl> Methods => extendsDecls.SelectMany(x => x.Methods);
    }
}