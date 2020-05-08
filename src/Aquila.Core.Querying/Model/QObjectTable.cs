using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;


namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Таблица объекта
    /// </summary>
    public partial class QObjectTable
    {
        private List<QField> _fields;

        private List<QTable> _tables;


        public QObjectTable(IPType type)
        {
            ObjectType = type;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public IPType ObjectType { get; }

        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= ObjectType.Properties.Select(x => (QField) new QSourceFieldExpression(this, x))
                .ToList();
        }

        public override IEnumerable<QTable> GetTables()
        {
            return _tables ??= ObjectType.Tables.Select(x => new QTable(this, x))
                .ToList();
        }
        public override string ToString()
        {
            return "Object: " + ObjectType.Name;
        }
    }


    public partial class QTable
    {
        private List<QField> _fields;
        
        public string Name => Table.Name;
        
        public override IEnumerable<QField> GetFields()
        {
            return _fields ??= Table.Properties.Select(x => (QField) new QSourceFieldExpression(this, x))
                .ToList();
        }
    }
}