using System.Collections.Generic;

namespace ZenPlatform.Core.Querying.Model
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
            ScopedDataSources = new DataSourceList();
        }

        public QueryContext QueryContext;

        /// <summary>
        /// Область видимости имен данных
        /// </summary>
        public Dictionary<string, QDataSource> Scope { get; set; }

        public DataSourceList ScopedDataSources { get; set; }
    }
}