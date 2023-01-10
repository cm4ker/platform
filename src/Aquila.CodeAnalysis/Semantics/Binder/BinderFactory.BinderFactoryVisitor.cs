#nullable disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Declarations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Aquila.CodeAnalysis.Semantics
{
    internal sealed partial class BinderFactory
    {
        internal class BinderFactoryVisitor : AquilaSyntaxVisitor<Binder>
        {
            private Stack<Binder> _binderHierarcy;

            private Binder TopStack
            {
                get => _binderHierarcy.TryPeek(out var res) ? res : null;
            }

            private MergedSourceCode _merged;
            private readonly AquilaCompilation _compilation;
            private readonly GlobalBinder _global;
            private int _position;
            private AquilaSyntaxNode _memberDeclarationOpt;
            private Symbol _memberOpt;
            private readonly BinderFactory _factory;

            private ConcurrentCache<BinderCacheKey, Binder> binderCache
            {
                get { return _factory._binderCache; }
            }

            private BuckStopsHereBinder buckStopsHereBinder
            {
                get { return _factory._buckStopsHereBinder; }
            }

            public BinderFactoryVisitor(BinderFactory factory)
            {
                _factory = factory;
                _compilation = factory._compilation;
                _merged = _compilation.SourceSymbolCollection.GetMergedSourceCode();

                _binderHierarcy = new Stack<Binder>();

                _global = new GlobalBinder(_compilation.SourceModule.GlobalNamespace, buckStopsHereBinder);

                _binderHierarcy.Push(_global);
            }

            internal void Initialize(int position, AquilaSyntaxNode memberDeclarationOpt, Symbol memberOpt)
            {
                Debug.Assert((memberDeclarationOpt == null) == (memberOpt == null));

                _position = position;
                _memberDeclarationOpt = memberDeclarationOpt;
                _memberOpt = memberOpt;
            }

            public void Push(Binder binder)
            {
                Debug.Assert(binder != null);
                _binderHierarcy.Push(binder);
            }

            public bool HasUp => _binderHierarcy.Any();

            public Binder Pop()
            {
                return _binderHierarcy.Pop();
            }

            public Binder Peek()
            {
                return _binderHierarcy.Peek();
            }

            private Binder CreateBinder(AquilaSyntaxNode element)
            {
                var next = Visit(element.Parent);

                switch (element)
                {
                    case ComponentDecl comDecl:
                    {
                        var ns = _compilation.PlatformSymbolCollection.GetNamespace(comDecl.Name.GetUnqualifiedName()
                            .Identifier
                            .Text);

                        return new InContainerBinder(ns, next);
                        ;
                    }
                    case ExtendDecl ext:
                    {
                        var types = next.Container.GetTypeMembers(ext.Name.GetUnqualifiedName().Identifier.Text);

                        if (Roslyn.Utilities.EnumerableExtensions.IsSingle(types))
                        {
                            return new InContainerBinder(types[0], next);
                            ;
                        }

                        break;
                    }
                    case HtmlMarkupDecl:
                    {
                        var container = next.Container as SourceViewTypeSymbol;
                        if (container == null)
                            throw new InvalidOperationException("Can't resolve ViewComponent symbol");

                        var builder = container.GetMembers().OfType<SourceViewTypeSymbol.MethodTreeBuilderSymbol>()
                            .Single();
                        return new InMethodBinder(builder, next);
                    }
                    case HtmlDecl:
                    {
                        var type = _compilation.SourceSymbolCollection.GetViewTypes().Single();
                        return new InContainerBinder(type, next);
                    }
                    case ImportDecl import:
                        break;
                    case FuncDecl fd:
                    {
                        NamespaceOrTypeSymbol container;

                        //TODO: visibility

                        if (fd.FuncOwner != null)
                        {
                            //check we need describe above the part type of this type

                            var hasDeclaredType = ((CompilationUnitSyntax)fd.Parent).Types.Any(x =>
                                x.Name.GetUnqualifiedName().Identifier.Text == fd.FuncOwner.OwnerType.ToString());

                            container = next.BindType(fd.FuncOwner.OwnerType);
                        }
                        else if (fd.IsGlobal)
                            container = next.Container;
                        else
                            container = next.Container;

                        var methods = container.GetMembers(fd.Identifier.Text).OfType<SourceMethodSymbol>();

                        MethodSymbol candidate = null;

                        if (!methods.Any())
                        {
                            candidate = new MissingMethodSymbol(name: fd.Identifier.Text);
                            return next;
                        }
                        else
                            //TODO: resolution overloads
                            candidate = methods.First();

                        return new InMethodBinder(candidate, next);
                        ;
                    }
                    case BlockStmt blc:
                    {
                        return new InMethodBinder(next.Method, next);
                    }
                }

                return next;
            }

            BinderCacheKey CreateBinderCacheKey(AquilaSyntaxNode node, NodeUsage usage)
            {
                return new BinderCacheKey(node, usage);
            }

            private void GetBinder(AquilaSyntaxNode element, out Binder binder)
            {
                var key = CreateBinderCacheKey(element, NodeUsage.Normal);

                if (!binderCache.TryGetValue(key, out binder))
                {
                    binder = CreateBinder(element);
                    binderCache.TryAdd(key, binder);
                }
            }

            public Binder VisitImportDecl(ImportDecl arg, Binder next)
            {
                var nsName = arg.Name.GetUnqualifiedName().Identifier.Text;

                var ns = _compilation.GlobalNamespace.GetMembers(nsName)
                    .OfType<NamespaceOrTypeSymbol>()
                    .ToImmutableArray();

                if (ns.Count() == 1)
                {
                    if (arg.ClrKeyword != default)
                    {
                        return new InClrImportBinder(ns.First(), next);
                    }
                    
                    return new InContainerBinder(ns.First(), next);
                }

                return new InContainerBinder(new MissingNamespaceSymbol(_compilation.GlobalNamespace, nsName),
                    next);
            }

            public override Binder? VisitCompilationUnit(CompilationUnitSyntax node)
            {
                var key = CreateBinderCacheKey(node, NodeUsage.Normal);

                Binder result;
                if (!binderCache.TryGetValue(key, out result))
                {
                    result = _global;

                    foreach (var importNode in node.Imports)
                    {
                        result = VisitImportDecl(importNode, result);
                    }

                    var module = node.Module;
                    if (module == null)
                        module = SyntaxFactory.ModuleDecl(SyntaxFactory.IdentifierName("main"));

                    result = VisitModuleDecl(module, result);
                    binderCache.TryAdd(key, result);
                }

                return result;
            }

            public Binder? VisitModuleDecl(ModuleDecl node, Binder next)
            {
                Assert.True(HasUp);

                var nextBinder = next;

                var ns = _compilation.GlobalNamespace.GetMembers(node.Name.GetUnqualifiedName().Identifier.Text)
                    .OfType<NamespaceOrTypeSymbol>()
                    .ToImmutableArray();

                if (ns.Count() == 1)
                {
                    return new InContainerBinder(ns.First(), nextBinder);
                }

                throw new Exception(
                    $"Internal semantic graph corrupted. Can't find module symbol '{node.Name.GetUnqualifiedName().Identifier.Text}'");
            }

            public override Binder VisitHtmlDecl(HtmlDecl arg)
            {
                GetBinder(arg, out var binder);
                return binder;
            }

            public override Binder VisitHtmlMarkupDecl(HtmlMarkupDecl arg)
            {
                GetBinder(arg, out var binder);
                return binder;
            }

            public override Binder VisitMethodDecl(MethodDecl arg)
            {
                GetBinder(arg, out var binder);
                return binder;
            }

            public override Binder? VisitFuncDecl(FuncDecl node)
            {
                GetBinder(node, out var binder);
                return binder;
            }

            public override Binder VisitExtendDecl(ExtendDecl arg)
            {
                GetBinder(arg, out var binder);
                return binder;
            }

            public override Binder VisitComponentDecl(ComponentDecl arg)
            {
                GetBinder(arg, out var binder);
                return binder;
            }

            public override Binder? DefaultVisit(SyntaxNode node)
            {
                return Visit(node.Parent);
            }
        }
    }
}