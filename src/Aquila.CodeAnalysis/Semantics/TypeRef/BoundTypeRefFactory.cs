using System;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.TypeRef;
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
            VoidTypeRef,
            BoolTypeRef,
            IntTypeRef,
            LongTypeRef,
            DoubleTypeRef,
            StringTypeRef,
            ObjectTypeRef;

        #endregion

        /// <summary>
        /// Initializes new instance of <see cref="BoundTypeRefFactory"/>.
        /// </summary>
        /// <param name="compilation">Bound compilation.</param>
        public BoundTypeRefFactory(AquilaCompilation compilation)
        {
            VoidTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Void, compilation.CoreTypes.Void.Symbol);
            BoolTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Boolean, compilation.CoreTypes.Boolean.Symbol);
            IntTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Int, compilation.CoreTypes.Int32.Symbol);
            LongTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Long, compilation.CoreTypes.Int64.Symbol);
            DoubleTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Double, compilation.CoreTypes.Double.Symbol);
            StringTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.String, compilation.CoreTypes.String.Symbol);
            ObjectTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Object, compilation.CoreTypes.Object.Symbol);
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
                case SpecialType.System_Int32: return IntTypeRef;
                case SpecialType.System_Int64: return LongTypeRef;
                case SpecialType.System_String: return StringTypeRef;
                case SpecialType.System_Single:
                case SpecialType.System_Double: return DoubleTypeRef;
                default:
                    throw new NotImplementedException();
            }
        }

        public BoundTypeRef CreateFromTypeRef(Aquila.Syntax.Ast.TypeRef tref, Binder binder = null, int arity = -1)
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

                return new BoundClassTypeRef(qName, binder?.Method, null, arity);
            }

            throw new NotImplementedException();
        }
    }
}