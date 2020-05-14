using System.Collections.Generic;
using Aquila.Core.Contracts.TypeSystem;


namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Оборачивает поле при смене источника данных, кроме вложенных запросов
    /// </summary>
    public partial class QIntermediateSourceField
    {
        public override string GetName()
        {
            return Field.GetName();
        }

        public override IEnumerable<IPType> GetExpressionType()
        {
            return Field.GetExpressionType();
        }

        public override string ToString()
        {
            return $"In-ate: {Field} From {DataSource}";
        }
    }
    
    /// <summary>
    /// Поле вложенного запроса
    /// </summary>
    public partial class QNestedQueryField
    {
        public override string GetName()
        {
            return Field.GetName();
        }

        public override IEnumerable<IPType> GetExpressionType()
        {
            return Field.GetExpressionType();
        }

        public override string ToString()
        {
            return $"Nested: {Field} From {DataSource}";
        }
    }
}