using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Таблица объекта
    /// </summary>
    public partial class QObjectTable : QDataSource
    {
        private List<QField> _fields;


        public QObjectTable(XCObjectTypeBase type)
        {
            ObjectType = type;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public XCObjectTypeBase ObjectType { get; }

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ObjectType.GetProperties().Select(x => (QField) new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override string? ToString()
        {
            return "Object: " + ObjectType.Name;
        }
    }
}