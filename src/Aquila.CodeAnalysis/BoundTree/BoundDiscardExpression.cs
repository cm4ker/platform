// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    internal partial class BoundDiscardExpression
    {
        public BoundExpression SetInferredTypeWithAnnotations(TypeWithAnnotations type)
        {
            Debug.Assert(Type is null && type.HasType);
            return this.Update(ValEscape, type.Type);
        }

        public BoundDiscardExpression FailInference(Binder binder, BindingDiagnosticBag? diagnosticsOpt)
        {
            if (diagnosticsOpt?.DiagnosticBag != null)
            {
                Binder.Error(diagnosticsOpt, ErrorCode.ERR_DiscardTypeInferenceFailed, this.Syntax);
            }
            return this.Update(ValEscape, binder.CreateErrorType("var"));
        }

        public override Symbol ExpressionSymbol
        {
            get
            {
                Debug.Assert(this.Type is { });
                return new DiscardSymbol(TypeWithAnnotations.Create(this.Type, this.TopLevelNullability.Annotation.ToInternalAnnotation()));
            }
        }
    }
}
