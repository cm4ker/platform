using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public class LTSelectExpression : LTField
    {
        public LTSelectExpression(LTQuery query)
        {
            Query = query;
        }

        /// <summary>
        /// Запрос-владелец
        /// </summary>
        public LTQuery Query { get; }

        /// <summary>
        /// Алиас выражения
        /// </summary>
        public string Aliase { get; set; }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Child.GetRexpressionType();
        }
    }
}