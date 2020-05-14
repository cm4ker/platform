using System;
using System.Collections.Generic;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Configuration.Structure.Data.Types;
using Aquila.Configuration.Structure.Data.Types.Primitive;

namespace Aquila.Core.Querying.Model
{
    public partial class QOperationExpression : QExpression
    {
        public override IEnumerable<IPType> GetExpressionType()
        {
            return Left.GetExpressionType();

            //throw new Exception("Return bool type");
            // yield return new XCBoolean();
            //yield break;
        }
    }
}