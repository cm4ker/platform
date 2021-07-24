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
        OrderExpression,
        ResultColumn
    }

    // public static class QLangExtensions
    // {
    //     public static IEnumerable<QTypeInfo> GetTypes(this SMProperty prop)
    //     {
    //         foreach (var type in prop.Types)
    //         {
    //             yield return type.Kind switch
    //             {
    //                 SMTypeKind.Int => new QTypeInfo(TypeKind.Int),
    //                 SMTypeKind.String => new QTypeInfo(TypeKind.String),
    //                 SMTypeKind.Reference => new QTypeInfo(TypeKind.Link),
    //                 _ => new QTypeInfo(TypeKind.Unknown)
    //             };
    //         }
    //     }
    // }
}