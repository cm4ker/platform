using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Кейс
    /// </summary>
    public partial class QCase : QExpression
    {
        public override IEnumerable<IXCType> GetExpressionType()
        {
            foreach (var when in Whens)
            {
                foreach (var typeBase in when.Then.GetExpressionType())
                {
                    yield return typeBase;
                }
            }

            foreach (var typeBase in Else.GetExpressionType())
            {
                yield return typeBase;
            }
        }
    }


    public partial class QWhen : QItem
    {
    }
}