using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aquila.Metadata;

namespace Aquila.Core.Querying.Model
{
    public enum QObjectType
    {
        FieldList,
        DataSourceList,
        WhenList,
        ExpressionList,
        JoinList,
        QueryList,
        OrderList,
        GroupList,
        OrderExpression,
        GroupExpression,
        ResultColumn
    }
}