﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Pchp.CodeAnalysis.CodeGen;
using Pchp.CodeAnalysis.FlowAnalysis;
using Pchp.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Pchp.CodeAnalysis;
using Pchp.CodeAnalysis.Semantics;
using Roslyn.Utilities;


namespace Aquila.CodeAnalysis.Semantics
{
    class BoundTypeRefFactory
    {
        #region Primitive Types

        internal readonly BoundPrimitiveTypeRef /*!*/
            VoidTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Void);

        internal readonly BoundPrimitiveTypeRef /*!*/
            NullTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Null);

        internal readonly BoundPrimitiveTypeRef /*!*/
            BoolTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Boolean);

        internal readonly BoundPrimitiveTypeRef /*!*/
            LongTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Long);

        internal readonly BoundPrimitiveTypeRef /*!*/
            DoubleTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Double);

        internal readonly BoundPrimitiveTypeRef /*!*/
            StringTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.String);

        internal readonly BoundPrimitiveTypeRef /*!*/
            ObjectTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Object);

        internal readonly BoundPrimitiveTypeRef /*!*/
            WritableStringRef = new BoundPrimitiveTypeRef(PhpTypeCode.WritableString);

        internal readonly BoundPrimitiveTypeRef /*!*/
            ArrayTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.PhpArray);

        internal readonly BoundPrimitiveTypeRef /*!*/
            IterableTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Iterable);

        internal readonly BoundPrimitiveTypeRef /*!*/
            CallableTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Callable);

        internal readonly BoundPrimitiveTypeRef /*!*/
            ResourceTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Resource);

        internal readonly BoundPrimitiveTypeRef /*!*/
            MixedTypeRef = new BoundPrimitiveTypeRef(PhpTypeCode.Mixed);

        #endregion

        #region Special Types

        internal readonly BoundClassTypeRef /*!*/
            TraversableTypeRef = new BoundClassTypeRef(NameUtils.SpecialNames.Traversable, null, null);

        internal readonly BoundClassTypeRef /*!*/
            ClosureTypeRef = new BoundClassTypeRef(NameUtils.SpecialNames.Closure, null, null);

        #endregion

        /// <summary>
        /// Initializes new instance of <see cref="BoundTypeRefFactory"/>.
        /// </summary>
        /// <param name="compilation">Bound compilation.</param>
        public BoundTypeRefFactory(PhpCompilation compilation)
        {
            // TBD
        }

        public BoundTypeRef Create(ITypeSymbol symbol)
        {
            if (symbol != null)
            {
                switch (symbol.SpecialType)
                {
                    case SpecialType.System_Void: return VoidTypeRef;
                    case SpecialType.System_Boolean: return BoolTypeRef;
                    case SpecialType.System_Int64: return LongTypeRef;
                    case SpecialType.System_Double: return DoubleTypeRef;
                    case SpecialType.System_String: return StringTypeRef;
                    case SpecialType.System_Object: return ObjectTypeRef;
                    default:

                        if (symbol.Is_PhpArray()) return ArrayTypeRef;
                        if (symbol.Is_PhpString()) return WritableStringRef;

                        return new BoundTypeRefFromSymbol(symbol);
                }
            }
            else
            {
                return null;
            }
        }

        public BoundTypeRef Create(ConstantValue c)
        {
            Contract.ThrowIfNull(c);

            switch (c.SpecialType)
            {
                case SpecialType.System_Boolean: return BoolTypeRef;
                case SpecialType.System_Int32:
                case SpecialType.System_Int64: return LongTypeRef;
                case SpecialType.System_String: return StringTypeRef;
                case SpecialType.System_Single:
                case SpecialType.System_Double: return DoubleTypeRef;
                // case SpecialType.System_Array: return WritableStringRef; // TODO: only array of bytes/chars
                default:
                    if (c.IsNull) return NullTypeRef;
                    throw new NotImplementedException();
            }
        }

        /// <summary>Create type reference refering to a variable containing <c>PhpTypeInfo</c> value.</summary>
        public static BoundTypeRef CreateFromPlace(IPlace place) => new BoundTypeRefFromPlace(place);

        public BoundTypeRef CreateFromTypeRef(TypeRef tref, SemanticsBinder binder = null,
            object self = null, bool objectTypeInfoSemantic = false, int arity = -1)
        {
            if (tref is PredefinedTypeRef pt)
            {
                switch (pt.Kind)
                {
                    case SyntaxKind.IntKeyword: return LongTypeRef;
                    case SyntaxKind.VoidKeyword: return VoidTypeRef;
                    case SyntaxKind.ObjectKeyword: return ObjectTypeRef;
                    case SyntaxKind.StringKeyword: return StringTypeRef;
                    
                    default: throw ExceptionUtilities.UnexpectedValue(pt.Kind);
                }
            }
            // else if (tref is Ast.INamedTypeRef named)
            // {
            //     if (named.ClassName == NameUtils.SpecialNames.System_Object) return ObjectTypeRef;
            //     //if (named.ClassName == NameUtils.SpecialNames.stdClass) return StdClassTypeRef;
            //
            //     if (named is Ast.TranslatedTypeRef tt && self != null &&
            //         tt.OriginalType is Ast.ReservedTypeRef reserved)
            //     {
            //         // keep self,parent,static not translated - better in cases where the type is ambiguous
            //         return CreateFromTypeRef(reserved, binder, self, objectTypeInfoSemantic);
            //     }
            //
            //     return new BoundClassTypeRef(named.ClassName, binder?.Routine, self ?? binder?.Self, arity);
            // }
            // else if (tref is Ast.ReservedTypeRef reserved) return new BoundReservedTypeRef(reserved.Type, self);
            // else if (tref is Ast.AnonymousTypeRef at)
            //     return new BoundTypeRefFromSymbol(at.TypeDeclaration.GetProperty<SourceTypeSymbol>());
            // else if (tref is Ast.MultipleTypeRef mt)
            // {
            //     return new BoundMultipleTypeRef(Create(mt.MultipleTypes, binder, self));
            // }
            // else if (tref is Ast.NullableTypeRef nullable)
            // {
            //     var t = CreateFromTypeRef(nullable.TargetType, binder, self, objectTypeInfoSemantic);
            //     if (t.IsNullable != true)
            //     {
            //         if (t is BoundPrimitiveTypeRef bpt)
            //         {
            //             // do not change the cached singleton // https://github.com/peachpiecompiler/peachpie/issues/455
            //             t = new BoundPrimitiveTypeRef(bpt.TypeCode);
            //         }
            //
            //         t.IsNullable = true;
            //     }
            //
            //     return t;
            // }
            // else if (tref is Ast.GenericTypeRef gt)
            // {
            //     return new BoundGenericClassTypeRef(
            //         CreateFromTypeRef(gt.TargetType, binder, self, objectTypeInfoSemantic,
            //             arity: gt.GenericParams.Count),
            //         Create(gt.GenericParams, binder, self));
            // }
            // else if (tref is Ast.IndirectTypeRef it)
            // {
            //     return new BoundIndirectTypeRef(
            //         binder.BindWholeExpression(it.ClassNameVar, BoundAccess.Read).SingleBoundElement(),
            //         objectTypeInfoSemantic);
            // }
            // else
            // {
            //     throw ExceptionUtilities.UnexpectedValue(tref);
            // }
        
            throw new NotImplementedException();
        }

        // ImmutableArray<BoundTypeRef> Create(IList<TypeRef> trefs, SemanticsBinder binder, SourceTypeSymbol self)
        // {
        //     return trefs.SelectAsArray(t =>
        //         CreateFromTypeRef(t, binder, self, objectTypeInfoSemantic: false).WithSyntax(t));
        // }
        //
        // public static IBoundTypeRef Create(QualifiedName qname, SourceTypeSymbol self) =>
        //     new BoundClassTypeRef(qname, null, self);
    }
}