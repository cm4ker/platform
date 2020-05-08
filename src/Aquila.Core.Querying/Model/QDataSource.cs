using System.Collections.Generic;
using System.Linq;

namespace Aquila.Core.Querying.Model
{
    public partial class QDataSource : QItem
    {
        public virtual IEnumerable<QField> GetFields()
        {
            return null;
        }

        public virtual IEnumerable<QTable> GetTables()
        {
            return null;
        }

        public QTable FindTable(string name)
        {
            return GetTables().FirstOrDefault(x => x.Name == name);
        }
    }
}