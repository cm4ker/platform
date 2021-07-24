using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;
using Aquila.CodeAnalysis.Semantics.Model;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Declarations;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Semantics
{
    internal class BinderFactory : AstWalker<Binder>
    {
        private readonly Dictionary<LangElement, Binder> _resolvedBinders;
        private Stack<Binder> _stack;

        private Binder TopStack
        {
            get => _stack.TryPeek(out var res) ? res : null;
        }

        private MergedSourceUnit _merged;
        private readonly AquilaCompilation _compilation;

        public BinderFactory(MergedSourceUnit merged, AquilaCompilation compilation)
        {
            _merged = merged;
            _compilation = compilation;
            _stack = new Stack<Binder>();

            _stack.Push(new GlobalBinder());
        }

        public void Push(Binder binder)
        {
            Debug.Assert(binder != null);
            _stack.Push(binder);
        }

        public Binder Pop()
        {
            return _stack.Pop();
        }

        public Binder CreateBinder(LangElement element)
        {
            if (_resolvedBinders.TryGetValue(element, out var binder))
                return binder;

            binder = Visit(element);
            _resolvedBinders.Add(element, binder);

            return binder;
        }

        public override Binder VisitSourceUnit(SourceUnit arg)
        {
            var unitBinder = new Binder(TopStack);
            _resolvedBinders.Add(arg, unitBinder);

            Push(unitBinder);

            base.VisitSourceUnit(arg);

            Pop();

            return unitBinder;
        }

        public override Binder VisitComponentDecl(ComponentDecl arg)
        {
            return base.VisitComponentDecl(arg);
        }

        public override Binder VisitExtendDecl(ExtendDecl arg)
        {
            return base.VisitExtendDecl(arg);
        }
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
                
                
-global
    - import Reference;
    
    - static int global_function()
    
    - component Document
        
        - extend Invoice
            
            - void SetSum()
               {
                    Order d = new Order(); 
                        ^
                    This is type founded in "InNamespaceContainer"
                }   
                
Binder
    -> GetNext() - getting next Binder for the lookup
    -> Container - TypeOrNamespace Context
         */
    }

    internal class GlobalBinder : Binder
    {
        public GlobalBinder() : base(null)
        {
        }
    }
}