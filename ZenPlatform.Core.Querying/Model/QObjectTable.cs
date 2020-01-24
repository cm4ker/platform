using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;


namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Таблица объекта
    /// </summary>
    public partial class QObjectTable : QDataSource
    {
        private List<QField> _fields;


        public QObjectTable(IType type)
        {
            ObjectType = type;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public IType ObjectType { get; }

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ObjectType.Properties.Select(x => (QField) new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override string ToString()
        {
            return "Object: " + ObjectType.Name;
        }
    }
}