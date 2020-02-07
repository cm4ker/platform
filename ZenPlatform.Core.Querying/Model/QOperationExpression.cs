using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

namespace ZenPlatform.Core.Querying.Model
{
    public partial class QOperationExpression : QExpression
    {
        public override IEnumerable<IPType> GetExpressionType()
        {
            throw new Exception("Return bool type");
           // yield return new XCBoolean();
        }
    }
}