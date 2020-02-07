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


        public QObjectTable(IPType ipType)
        {
            ObjectIpType = ipType;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public IPType ObjectIpType { get; }

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ObjectIpType.Properties.Select(x => (QField) new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override string ToString()
        {
            return "Object: " + ObjectIpType.Name;
        }
    }
}