using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Таблица объекта
    /// </summary>
    public class QObjectTable : QItem, IQDataSource
    {
        public QObjectTable(XCObjectTypeBase type)
        {
            ObjectType = type;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public XCObjectTypeBase ObjectType { get; }

        public IEnumerable<QField> GetFields()
        {
            foreach (var prop in ObjectType.GetProperties())
            {
                yield return new QSourceFieldExpression(this, prop);
            }
        }
    }
}