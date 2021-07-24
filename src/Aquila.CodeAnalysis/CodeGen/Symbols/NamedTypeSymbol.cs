﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Symbols
{
    partial class NamedTypeSymbol
    {
        /// <summary>
        /// Provides information about a method and its override.
        /// </summary>
        [DebuggerDisplay("{Method.ContainingType,nq} {Method.MethodName,nq}")]
        internal struct OverrideInfo
        {
            /// <summary>
            /// Method to be overriden.
            /// </summary>
            public MethodSymbol Method { get; set; }

            /// <summary>
            /// Gets the method name.
            /// </summary>
            public string MethodName => Method.MethodName;

            /// <summary>
            /// The method is abstract with no possible override.
            /// </summary>
            public bool IsUnresolvedAbstract => Method.IsAbstract && !HasOverride;

            /// <summary>
            /// Whether there is a possible override of <see cref="Method"/>.
            /// </summary>
            public bool HasOverride => Override != null || OverrideCandidate != null;

            /// <summary>
            /// Whether the override resolves implementation of a newly introduced interface method.
            /// </summary>
            public bool ImplementsInterface { get; set; }

            /// <summary>
            /// Metched override.
            /// </summary>
            public MethodSymbol Override { get; }

            /// <summary>
            /// A candidate override which signature does not match exactly the method.
            /// </summary>
            public MethodSymbol OverrideCandidate { get; }

            public OverrideInfo(MethodSymbol method, MethodSymbol methodoverride = null)
            {
                Debug.Assert(method != null);

                this.Method = method;
                this.Override = null;
                this.OverrideCandidate = null;
                this.ImplementsInterface = false;

                //
                // store the override,
                // either as an override or just a candidate if the signatures are not matching.
                // In case of the candidate, a ghost stub will be generated later.
                //

                if (methodoverride != null)
                {
                    if (methodoverride.IsExplicitInterfaceImplementation(method) ||
                        (methodoverride.SignaturesMatch(method) && methodoverride.IsVirtual))
                    {
                        // overrides:
                        this.Override = methodoverride;
                    }
                    else
                    {
                        this.OverrideCandidate = methodoverride;
                    }
                }
            }
        }

        OverrideInfo[] _lazyOverrides;

        /// <summary>
        /// Matches all methods that can be overriden (non-static, public or protected, abstract or virtual)
        /// within this type sub-tree (this type, its base and interfaces)
        /// with its override.
        /// Methods without an override are either abstract or a ghost stup has to be synthesized.
        /// </summary>
        /// <param name="diagnostics"></param>
        internal OverrideInfo[] ResolveOverrides(DiagnosticBag diagnostics)
        {
            if (_lazyOverrides != null)
            {
                // already resolved
                return _lazyOverrides;
            }

            // inherit abstracts from base type
            var overrides = new List<OverrideInfo>();
            if (BaseType != null && BaseType.SpecialType != SpecialType.System_Object)
            {
                overrides.AddRange(BaseType.ResolveOverrides(diagnostics));
            }

            // collect this type declared methods including synthesized methods
            var members = this.GetMembers();

            // resolve overrides of inherited members
            for (int i = 0; i < overrides.Count; i++)
            {
                var m = overrides[i];
                if (m.HasOverride == false)
                {
                    // update override info of the inherited member
                    overrides[i] = new OverrideInfo(m.Method,
                        OverrideHelper.ResolveMethodImplementation(m.Method, members));
                }
                else
                {
                    // clear the interface flag of inherited override info
                    m.ImplementsInterface = false;
                    overrides[i] = m;
                }
            }

            // resolve overrides of interface methods
            var declaredifaces = GetDeclaredInterfaces(null);
            foreach (var iface in declaredifaces)
            {
                // skip interfaces implemented by base type or other interfaces,
                // we don't want to add redundant override entries:
                if (BaseType?.ImplementsInterface(iface) == true ||
                    declaredifaces.Any(x => x != iface && x.ImplementsInterface(iface)))
                {
                    // iface is already handled within overrides => skip
                    // note: iface can be ignored in metadata at all actually
                    continue;
                }

                var iface_abstracts = iface.ResolveOverrides(diagnostics);
                foreach (var m in iface_abstracts)
                {
                    if (BaseType != null && m.Method.ContainingType != iface &&
                        BaseType.ImplementsInterface(m.Method.ContainingType))
                    {
                        // iface {m.Method.ContainingType} already handled within overrides => skip
                        continue;
                    }

                    // ignore interface method that is already implemented:
                    if (overrides.Any(o => OverrideHelper.SignaturesMatch(o.Method, m.Method)))
                    {
                        continue;
                    }

                    // add interface member,
                    // resolve its override
                    overrides.Add(new OverrideInfo(m.Method,
                            this.IsInterface ? null : OverrideHelper.ResolveMethodImplementation(m.Method, this))
                        { ImplementsInterface = true });
                }
            }

            // add overrideable methods from this type
            foreach (var s in members)
            {
                if (s is MethodSymbol m && m.IsOverrideable())
                {
                    overrides.Add(new OverrideInfo(m));
                }
            }

            // handle unresolved abstracts
            for (int i = 0; i < overrides.Count; i++)
            {
                var m = overrides[i];
            }

            // cache & return
            return (_lazyOverrides = overrides.ToArray());
        }
    }
}