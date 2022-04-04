// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aquila.CodeAnalysis.Errors;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Represents an event.
    /// </summary>
    internal abstract partial class EventSymbol : Symbol
    {
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Changes to the public interface of this class should remain synchronized with the VB version.
        // Do not make any changes to the public interface without making the corresponding change
        // to the VB version.
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        internal EventSymbol()
        {
        }

        /// <summary>
        /// The original definition of this symbol. If this symbol is constructed from another
        /// symbol by type substitution then OriginalDefinition gets the original symbol as it was defined in
        /// source or metadata.
        /// </summary>
        public new virtual EventSymbol OriginalDefinition
        {
            get { return this; }
        }

        protected sealed override Symbol OriginalSymbolDefinition
        {
            get { return this.OriginalDefinition; }
        }

        /// <summary>
        /// The type of the event along with its annotations.
        /// </summary>
        public abstract TypeWithAnnotations TypeWithAnnotations { get; }

        /// <summary>
        /// The type of the event.
        /// </summary>
        public TypeSymbol Type => TypeWithAnnotations.Type;

        /// <summary>
        /// The 'add' accessor of the event.  Null only in error scenarios.
        /// </summary>
        public abstract MethodSymbol? AddMethod { get; }

        /// <summary>
        /// The 'remove' accessor of the event.  Null only in error scenarios.
        /// </summary>
        public abstract MethodSymbol? RemoveMethod { get; }

        internal bool HasAssociatedField
        {
            get { return (object?)this.AssociatedField != null; }
        }

        /// <summary>
        /// Returns true if this symbol requires an instance reference as the implicit receiver. This is false if the symbol is static.
        /// </summary>
        public virtual bool RequiresInstanceReceiver => !IsStatic;

        /// <summary>
        /// True if this is a Windows Runtime-style event.
        /// 
        /// A normal C# event, "event D E", has accessors
        ///     void add_E(D d)
        ///     void remove_E(D d)
        /// 
        /// A Windows Runtime event, "event D E", has accessors
        ///     EventRegistrationToken add_E(D d)
        ///     void remove_E(EventRegistrationToken t)
        /// </summary>
        public abstract bool IsWindowsRuntimeEvent { get; }

        /// <summary>
        /// True if the event itself is excluded from code coverage instrumentation.
        /// True for source events marked with <see cref="ExcludeFromCodeCoverageAttribute"/>.
        /// </summary>
        internal virtual bool IsDirectlyExcludedFromCodeCoverage
        {
            get => false;
        }

        /// <summary>
        /// True if this symbol has a special name (metadata flag SpecialName is set).
        /// </summary>
        internal abstract bool HasSpecialName { get; }

        /// <summary>
        /// Gets the attributes on event's associated field, if any.
        /// Returns an empty <see cref="ImmutableArray&lt;AttributeData&gt;"/> if
        /// there are no attributes.
        /// </summary>
        /// <remarks>
        /// This publicly exposes the attributes of the internal backing field.
        /// </remarks>
        public ImmutableArray<AttributeData> GetFieldAttributes()
        {
            return (object?)this.AssociatedField == null
                ? ImmutableArray<AttributeData>.Empty
                : this.AssociatedField.GetAttributes();
        }

        internal virtual FieldSymbol? AssociatedField
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the overridden event, or null.
        /// </summary>
        public EventSymbol? OverriddenEvent
        {
            get
            {
                if (this.IsOverride)
                {
                    if (IsDefinition)
                    {
                        return (EventSymbol)OverriddenOrHiddenMembers.GetOverriddenMember();
                    }

                    return (EventSymbol)OverriddenOrHiddenMembersResult.GetOverriddenMember(this,
                        OriginalDefinition.OverriddenEvent);
                }

                return null;
            }
        }

        internal virtual OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers
        {
            get
            {
                throw new NotImplementedException();

                //return this.MakeOverriddenOrHiddenMembers();
            }
        }

        internal bool HidesBaseEventsByName
        {
            get
            {
                MethodSymbol? accessor = AddMethod ?? RemoveMethod;
                return (object?)accessor != null && accessor.HidesBaseMethodsByName;
            }
        }

        internal EventSymbol GetLeastOverriddenEvent(NamedTypeSymbol? accessingTypeOpt)
        {
            accessingTypeOpt = accessingTypeOpt?.OriginalDefinition;
            EventSymbol e = this;
            while (e.IsOverride && !e.HidesBaseEventsByName)
            {
                // NOTE: We might not be able to access the overridden event. For example,
                // 
                //   .assembly A
                //   {
                //      InternalsVisibleTo("B")
                //      public class A { internal virtual event Action E { add; remove; } }
                //   }
                // 
                //   .assembly B
                //   {
                //      InternalsVisibleTo("C")
                //      public class B : A { internal override event Action E { add; remove; } }
                //   }
                // 
                //   .assembly C
                //   {
                //      public class C : B { ... new B().E += null ... }       // A.E is not accessible from here
                //   }
                //
                // See InternalsVisibleToAndStrongNameTests: IvtVirtualCall1, IvtVirtualCall2, IvtVirtual_ParamsAndDynamic.
                EventSymbol? overridden = e.OverriddenEvent;
                var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                // if ((object?)overridden == null ||
                //     (accessingTypeOpt is { } && !AccessCheck.IsSymbolAccessible(overridden, accessingTypeOpt, ref discardedUseSiteInfo)))
                // {
                //     break;
                // }

                e = overridden;
            }

            return e;
        }

        /// <summary>
        /// Source: Was the member name qualified with a type name?
        /// Metadata: Is the member an explicit implementation?
        /// </summary>
        /// <remarks>
        /// Will not always agree with ExplicitInterfaceImplementations.Any()
        /// (e.g. if binding of the type part of the name fails).
        /// </remarks>
        internal virtual bool IsExplicitInterfaceImplementation
        {
            get { return ExplicitInterfaceImplementations.Any(); }
        }

        /// <summary>
        /// Returns interface events explicitly implemented by this event.
        /// </summary>
        /// <remarks>
        /// Events imported from metadata can explicitly implement more than one event.
        /// </remarks>
        public abstract ImmutableArray<EventSymbol> ExplicitInterfaceImplementations { get; }

        /// <summary>
        /// Gets the kind of this symbol.
        /// </summary>
        public sealed override SymbolKind Kind
        {
            get { return SymbolKind.Event; }
        }

        internal EventSymbol AsMember(NamedTypeSymbol newOwner)
        {
            Debug.Assert(this.IsDefinition);
            Debug.Assert(ReferenceEquals(newOwner.OriginalDefinition, this.ContainingSymbol.OriginalDefinition));
            Debug.Assert(newOwner.IsDefinition || newOwner is SubstitutedNamedTypeSymbol);
            return newOwner.IsDefinition ? this : throw new NotImplementedException();
        }

        internal abstract bool MustCallMethodsDirectly { get; }


        #region Equality

        public override bool Equals(Symbol? obj, TypeCompareKind compareKind)
        {
            EventSymbol? other = obj as EventSymbol;

            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // This checks if the events have the same definition and the type parameters on the containing types have been
            // substituted in the same way.
            return TypeSymbol.Equals(this.ContainingType, other.ContainingType, compareKind) &&
                   ReferenceEquals(this.OriginalDefinition, other.OriginalDefinition);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = Hash.Combine(this.ContainingType, hash);
            hash = Hash.Combine(this.Name, hash);
            return hash;
        }

        #endregion Equality
    }
}