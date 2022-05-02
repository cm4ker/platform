using System;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;

namespace Aquila.CodeAnalysis;

/*
 
 var a;
 
 using a.block
 {
     var b = def_var(type, default_value);
     assign(b, b.access_member("test").call());
     a.block.this
 }
 
 
 
 */

public class BoundTreeGenerator
{
    class VarHelper
    {
        public VarHelper(BoundVariableName boundVar)
        {
        }
    }

    class BoundBlockHelper : IDisposable
    {
        private readonly MethodSymbol _owner;

        public BoundBlockHelper(MethodSymbol owner)
        {
            _owner = owner;
        }

        public BoundThisParameter This => new BoundThisParameter(_owner);

        public void Dispose()
        {
        }
    }

    public void Var()
    {
    }

    public void BinaryOp(object optype, object arg1, object arg2)
    {
    }

    public void UnaryOp(object optype, object arg1)
    {
    }

    public void Block()
    {
    }
}