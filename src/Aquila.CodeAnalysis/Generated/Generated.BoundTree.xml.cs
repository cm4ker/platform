using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    public abstract partial class BoundStatement : BoundOperation
    {
        public BoundStatement()
        {
        }

        public override OperationKind Kind
        {
            get;
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public virtual TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return default;
        }
    }
}

namespace Aquila.CodeAnalysis.Semantics.PrivateNS
{
    public abstract partial class BoundEmptyStatement : BoundStatement
    {
        public BoundEmptyStatement()
        {
        }

        public override OperationKind Kind
        {
            get;
        }

        partial void AcceptImpl<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument, ref TRes result);
        partial void AcceptImpl(OperationVisitor visitor);
        public override TRes Accept<TArg, TRes>(OperationVisitor<TArg, TRes> visitor, TArg argument)
        {
            TRes res = default;
            AcceptImpl(visitor, argument, ref res);
            return res;
        }

        public override void Accept(OperationVisitor visitor)
        {
            AcceptImpl(visitor);
        }

        public virtual TResult Accept<TResult>(AquilaOperationVisitor<TResult> visitor)
        {
            return default;
        }
    }
}
