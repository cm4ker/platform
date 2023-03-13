using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Utilities;
using EnumerableExtensions = Roslyn.Utilities.EnumerableExtensions;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// List of overloads for a function call.
    /// </summary>
    internal readonly struct OverloadsList
    {
        /// <summary>
        /// Defines the scope of members visibility.
        /// Used to resolve visibility of called methods and accessed properties.
        /// </summary>
        public readonly struct VisibilityScope
        {
            /// <summary>
            /// The type scope if resolved.
            /// Can be <c>null</c> when outside of class or when scope is unknown in compile-time.
            /// </summary>
            public readonly NamedTypeSymbol Scope;

            /// <summary>
            /// Whether the scope can change.
            /// In result visibility of private and protected members may change in runtime. 
            /// </summary>
            public readonly bool ScopeIsDynamic;

            /// <summary>
            /// Builds the visibility scope.
            /// </summary>
            public VisibilityScope(NamedTypeSymbol self, SourceMethodSymbolBase method)
            {
                Scope = self;
                ScopeIsDynamic = (method?.IsGlobalScope == true);
            }
        }

        [Flags]
        public enum InvocationKindFlags
        {
            InstanceCall = 1,
            StaticCall = 2,

            New = InstanceCall,
        }

        readonly MethodSymbol _single;
        readonly MethodSymbol[] _methods;

        public OverloadsList(MethodSymbol method)
        {
            _single = method ?? throw ExceptionUtilities.ArgumentNull();
            _methods = default;
        }

        public OverloadsList(MethodSymbol[] methods)
        {
            if (methods.Length == 1)
            {
                _single = methods[0];
                _methods = default;
            }
            else
            {
                _single = default;
                _methods = methods;
            }
        }

        /// <summary>
        /// Tries to resolve method in design time.
        /// </summary>
        /// <returns>
        /// Might return one of following:
        /// - resolved single <see cref="MethodSymbol"/>
        /// - <see cref="MissingMethodSymbol"/>
        /// - <see cref="AmbiguousMethodSymbol"/>
        /// - <see cref="InaccessibleMethodSymbol"/>
        /// </returns>
        public MethodSymbol Resolve(TypeRefContext typeCtx, ImmutableArray<BoundArgument> args, VisibilityScope scope,
            InvocationKindFlags flags)
        {
            if (_single != null)
            {
                if (IsAccessible(_single, scope))
                    return scope is { ScopeIsDynamic: true } && IsNonPublic(_single)
                        ? new AmbiguousMethodSymbol(ImmutableArray.Create(_single), false) 
                        : _single;
                return new InaccessibleMethodSymbol(ImmutableArray.Create(_single));
            }

            if (_methods == null || _methods.Length == 0)
            {
                return new MissingMethodSymbol();
            }

            // collect valid methods:
            var result = new List<MethodSymbol>(_methods.Where(MethodSymbolExtensions.IsValidMethod));

            // only visible methods:
            RemoveInaccessible(result, scope);

            if (result.Count == 0)
            {
                return new InaccessibleMethodSymbol(_methods.AsImmutable());
            }

            // if there are both instance and static methods,
            // take the right ones preferably:
            var statics = result.Count(m => m.IsStatic);
            if (statics > 0 && statics < result.Count)
            {
                if ((flags & InvocationKindFlags.StaticCall) == 0)
                {
                    result.RemoveAll(m => m.IsStatic);
                }

                if ((flags & InvocationKindFlags.InstanceCall) == 0)
                {
                    result.RemoveAll(m => !m.IsStatic);
                }
            }

            if (scope.ScopeIsDynamic && result.Any(IsNonPublic))
            {
                // we have to postpone the resolution to runtime:
                return new AmbiguousMethodSymbol(result.AsImmutable(), false);
            }

            if (result.Count == 1)
            {
                return result[0];
            }

            var m = EnumerableExtensions.WhereNotNull(result)
                .FirstOrDefault(m => args.Length >= 0 && args.Length <= m.Parameters.Length);
            
            return m != null ? m : new AmbiguousMethodSymbol(result.AsImmutable(), true);
        }

        private static bool IsNonPublic(MethodSymbol m) => m.DeclaredAccessibility != Accessibility.Public;

        /// <summary>
        /// Removes methods that are inaccessible for sure.
        /// </summary>
        static void RemoveInaccessible(List<MethodSymbol> methods, VisibilityScope scope)
        {
            for (int i = methods.Count - 1; i >= 0; i--)
            {
                if (!IsAccessible(methods[i], scope))
                {
                    methods.RemoveAt(i);
                }
            }
        }

        static bool IsAccessible(MethodSymbol m, VisibilityScope scope)
        {
            return (
                m.DeclaredAccessibility != Accessibility.ProtectedAndInternal && // C# 7.2 "private protected"
                m.DeclaredAccessibility != Accessibility.Internal && // "internal"
                (scope.ScopeIsDynamic ||
                 m.IsAccessible(scope.Scope)) && // method is accessible (or might be in runtime)
                !m.IsInitFieldsOnly && // method is not a special .ctor which is not accessible from user's code
                !m.IsAquilaHidden() // ignore hidden methods
            );
        }
    }
}