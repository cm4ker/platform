using System;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Syntax;
using Roslyn.Utilities;


namespace Aquila.CodeAnalysis.Semantics
{
    class BoundTypeRefFactory
    {
        #region Primitive Types

        internal readonly BoundPrimitiveTypeRef
            VoidTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Void);

        internal readonly BoundPrimitiveTypeRef
            NullTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Null);

        internal readonly BoundPrimitiveTypeRef
            BoolTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Boolean);

        internal readonly BoundPrimitiveTypeRef
            IntTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Int);

        internal readonly BoundPrimitiveTypeRef
            LongTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Long);

        internal readonly BoundPrimitiveTypeRef
            DoubleTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Double);

        internal readonly BoundPrimitiveTypeRef
            StringTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.String);

        internal readonly BoundPrimitiveTypeRef
            ObjectTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Object);

        internal readonly BoundPrimitiveTypeRef
            WritableStringRef = new BoundPrimitiveTypeRef(AquilaTypeCode.WritableString);

        internal readonly BoundPrimitiveTypeRef
            ArrayTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.AquilaArray);

        internal readonly BoundPrimitiveTypeRef
            IterableTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Iterable);

        internal readonly BoundPrimitiveTypeRef
            CallableTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Callable);

        internal readonly BoundPrimitiveTypeRef
            ResourceTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Resource);

        internal readonly BoundPrimitiveTypeRef
            MixedTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Mixed);

        #endregion

        #region Special Types

        internal readonly BoundClassTypeRef
            TraversableTypeRef = new BoundClassTypeRef(NameUtils.SpecialNames.Traversable, null);

        internal readonly BoundClassTypeRef
            ClosureTypeRef = new BoundClassTypeRef(NameUtils.SpecialNames.Closure, null);

        #endregion

        /// <summary>
        /// Initializes new instance of <see cref="BoundTypeRefFactory"/>.
        /// </summary>
        /// <param name="compilation">Bound compilation.</param>
        public BoundTypeRefFactory(AquilaCompilation compilation)
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
                    case SpecialType.System_Int32: return IntTypeRef;
                    case SpecialType.System_Double: return DoubleTypeRef;
                    case SpecialType.System_String: return StringTypeRef;
                    case SpecialType.System_Object: return ObjectTypeRef;
                    default:


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

        public BoundTypeRef CreateFromTypeRef(Aquila.Syntax.Ast.TypeRef tref, Binder1 binder = null,
            object self = null, bool objectTypeInfoSemantic = false, int arity = -1)
        {
            if (tref is PredefinedTypeRef pt)
            {
                switch (pt.Kind)
                {
                    case SyntaxKind.IntKeyword: return IntTypeRef;
                    case SyntaxKind.LongKeyword: return LongTypeRef;
                    case SyntaxKind.VoidKeyword: return VoidTypeRef;
                    case SyntaxKind.ObjectKeyword: return ObjectTypeRef;
                    case SyntaxKind.StringKeyword: return StringTypeRef;
                    case SyntaxKind.BoolKeyword: return BoolTypeRef;

                    default: throw ExceptionUtilities.UnexpectedValue(pt.Kind);
                }
            }
            else if (tref is NamedTypeRef named)
            {
                var qName = new QualifiedName(new Name(named.Value));

                if (qName == NameUtils.SpecialNames.System_Object) return ObjectTypeRef;

                // if (named is Ast.TranslatedTypeRef tt && self != null &&
                //     tt.OriginalType is Ast.ReservedTypeRef reserved)
                // {
                //     // keep self,parent,static not translated - better in cases where the type is ambiguous
                //     return CreateFromTypeRef(reserved, binder, self, objectTypeInfoSemantic);
                // }

                return new BoundClassTypeRef(qName, binder?.Method, arity);
            }
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