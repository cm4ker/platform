using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Aquila.CodeAnalysis.Semantics.TypeRef;
using Aquila.CodeAnalysis.Utilities;

namespace Aquila.CodeAnalysis.Semantics
{
    public partial class AquilaOperationVisitor<TResult>
    {
        /// <summary>Visits given operation.</summary>
        protected TResult Accept(IAquilaOperation x) => (x != null) ? x.Accept(this) : default;

        public virtual TResult VisitDefault(BoundOperation x) => default;
    }
}