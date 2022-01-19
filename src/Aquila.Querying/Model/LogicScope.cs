using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Metadata;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Querying.Model
{
    public enum QueryContext
    {
        None = 0,
        From = 1,
        GroupBy = 2,
        Having = 3,
        Where = 4,
        Select = 5,
        OrderBy = 6
    }

    public class LogicScope
    {
        public LogicScope()
        {
            Scope = new Dictionary<string, QDataSource>();
            ScopedDataSources = new List<QDataSource>();
            Criteria = new();
        }

        public QueryContext QueryContext;

        /// <summary>
        /// Область видимости имен данных
        /// </summary>
        public Dictionary<string, QDataSource> Scope { get; set; }

        public List<QDataSource> ScopedDataSources { get; set; }

        public List<QCriterion> Criteria { get; set; }
    }
}