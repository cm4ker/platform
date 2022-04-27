using System;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.FlowAnalysis;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Roslyn.Utilities;


namespace Aquila.CodeAnalysis.Semantics
{
    class PrimitiveBoundTypeRefs
    {
        #region Primitive Types

        // internal readonly BoundPrimitiveTypeRef
        //     VoidTypeRef,
        //     BoolTypeRef,
        //     IntTypeRef,
        //     LongTypeRef,
        //     DoubleTypeRef,
        //     StringTypeRef,
        //     ObjectTypeRef;

        #endregion

        // /// <summary>
        // /// Initializes new instance of <see cref="PrimitiveBoundTypeRefs"/>.
        // /// </summary>
        // /// <param name="compilation">Bound compilation.</param>
        // public PrimitiveBoundTypeRefs(AquilaCompilation compilation)
        // {
        //     VoidTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Void, compilation.CoreTypes.Void.Symbol);
        //     BoolTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Boolean, compilation.CoreTypes.Boolean.Symbol);
        //     IntTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Int, compilation.CoreTypes.Int32.Symbol);
        //     LongTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Long, compilation.CoreTypes.Int64.Symbol);
        //     DoubleTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Double, compilation.CoreTypes.Double.Symbol);
        //     StringTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.String, compilation.CoreTypes.String.Symbol);
        //     ObjectTypeRef = new BoundPrimitiveTypeRef(AquilaTypeCode.Object, compilation.CoreTypes.Object.Symbol);
        // }
    }
}