using System;
using System.Collections.Generic;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Shared.Tree;

namespace Aquila.Language.Ast.Definitions
{
    public partial class Root
    {
        private void AddUnit(CompilationUnit unit)
        {
            this.Units.Add(unit);
        }

        public override void Add(Node node)
        {
            AddUnit(node as CompilationUnit ?? throw new Exception("You can add only CompilationUnitNode"));
        }

        public void Add(CompilationUnit cu)
        {
            AddUnit(cu);
        }
    }
}