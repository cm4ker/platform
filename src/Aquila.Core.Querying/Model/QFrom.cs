using System.Collections.Generic;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Представляет собой логическую структуру запроса части запроса FROM
    /// </summary>
    public partial class QFrom : QItem
    {
        public override string ToString()
        {
            return "From";
        }
    }
}