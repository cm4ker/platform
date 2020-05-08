using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Кейс
    /// </summary>
    public partial class QCase : QExpression
    {
        public override IEnumerable<IPType> GetExpressionType()
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