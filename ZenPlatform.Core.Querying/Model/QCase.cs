using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Кейс
    /// </summary>
    public partial class QCase : QExpression
    {
        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            foreach (var when in Whens)
            {
                foreach (var typeBase in when.Then.GetRexpressionType())
                {
                    yield return typeBase;
                }

                foreach (var typeBase in when.Else.GetRexpressionType())
                {
                    yield return typeBase;
                }
            }
        }
    }


    public partial class QCaseWhen : QItem
    {
    }
}