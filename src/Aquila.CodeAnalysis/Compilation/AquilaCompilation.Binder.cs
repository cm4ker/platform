using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis;

public partial class AquilaCompilation
{
    // When building symbols from the declaration table (lazily), or inside a type, or when
    // compiling a method body, we may not have a BinderContext in hand for the enclosing
    // scopes.  Therefore, we build them when needed (and cache them) using a ContextBuilder.
    // Since a ContextBuilder is only a cache, and the identity of the ContextBuilders and
    // BinderContexts have no semantic meaning, we can reuse them or rebuild them, whichever is
    // most convenient.  We store them using weak references so that GC pressure will cause them
    // to be recycled.
    private WeakReference<BinderFactory>[]? _binderFactories;
    private WeakReference<BinderFactory>[]? _ignoreAccessibilityBinderFactories;

    internal BinderFactory GetBinderFactory(SyntaxTree syntaxTree, bool ignoreAccessibility = false)
    {
        return GetBinderFactory(syntaxTree, ignoreAccessibility: false, ref _binderFactories);
    }

    private BinderFactory GetBinderFactory(SyntaxTree syntaxTree, bool ignoreAccessibility,
        ref WeakReference<BinderFactory>[]? cachedBinderFactories)
    {
        Debug.Assert(System.Runtime.CompilerServices.Unsafe.AreSame(ref cachedBinderFactories,
            ref ignoreAccessibility ? ref _ignoreAccessibilityBinderFactories : ref _binderFactories));

        var treeNum = GetSyntaxTreeOrdinal(syntaxTree);
        WeakReference<BinderFactory>[]? binderFactories = cachedBinderFactories;
        if (binderFactories == null)
        {
            binderFactories = new WeakReference<BinderFactory>[this.SyntaxTrees.Length];
            binderFactories = Interlocked.CompareExchange(ref cachedBinderFactories, binderFactories, null) ??
                              binderFactories;
        }

        BinderFactory? previousFactory;
        var previousWeakReference = binderFactories[treeNum];
        if (previousWeakReference != null && previousWeakReference.TryGetTarget(out previousFactory))
        {
            return previousFactory;
        }

        return AddNewFactory(syntaxTree, ignoreAccessibility, ref binderFactories[treeNum]);
    }

    private BinderFactory AddNewFactory(SyntaxTree syntaxTree, bool ignoreAccessibility,
        ref WeakReference<BinderFactory>? slot)
    {
        var newFactory = new BinderFactory(this, syntaxTree, ignoreAccessibility);
        var newWeakReference = new WeakReference<BinderFactory>(newFactory);

        while (true)
        {
            BinderFactory? previousFactory;
            WeakReference<BinderFactory>? previousWeakReference = slot;
            if (previousWeakReference != null && previousWeakReference.TryGetTarget(out previousFactory))
            {
                Debug.Assert(slot is object);
                return previousFactory;
            }

            if (Interlocked.CompareExchange(ref slot!, newWeakReference, previousWeakReference) ==
                previousWeakReference)
            {
                return newFactory;
            }
        }
    }

    internal Binder GetBinder(AquilaSyntaxNode syntax)
    {
        return GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax);
    }
    
    
}