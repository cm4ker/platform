using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Functions;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Semantics
{
    internal class BinderFactory : AstWalker<Binder>
    {
        private readonly Dictionary<SourceUnit, object> _comDic;
        private Stack<Binder> _stack;

        private Binder Current
        {
            get => _stack.TryPeek(out var res) ? res : null;
        }
 
        public BinderFactory(Dictionary<SourceUnit, object> comDic)
        {
            _comDic = comDic;
            _stack = new Stack<Binder>();
        }

        public override Binder VisitSourceUnit(SourceUnit arg)
        {
            var component = _comDic[arg];
            _stack.Push(new Binder(Current));

            //Always we are expect underlaing binder
            return base.VisitSourceUnit(arg);
            ;
        }

        // public Binder VisitComponent()
        // {
        //     
        // }
    }

    internal class InContainerBinder : Binder
    {
        private readonly NamedTypeSymbol _container;

        public InContainerBinder(NamedTypeSymbol container, Binder next) : base(next)
        {
            _container = container;
        }

        public override NamespaceOrTypeSymbol Container => _container;
    }

    internal class Binder
    {
        private readonly Binder _next;

        public Binder(Binder next)
        {
            _next = next;
        }

        public virtual Binder GetNext()
        {
            return _next;
        }


        public virtual NamespaceOrTypeSymbol Container { get; }

        /*
Component
    - ComponentItem
        - Module
            -Code
                -Method
                
                
-Global Namespace
    - Component Namespace
        - Type
            - Method
                using Entity;
            
                void A()
                {
                    Document1 d = new Document1(); 
                        ^
                    1) Semantics know that this is a type
                    2) Lookup current type namespace container
                    3) Lookup 1st using declaration container, then second etc...
                }   
                
Binder
    -> GetNext() - getting next Binder for the lookup
    -> Container - TypeOrNamespace Context
         */
    }
}