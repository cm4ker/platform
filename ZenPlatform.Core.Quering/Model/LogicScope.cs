using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
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
            Scope = new Dictionary<string, IQDataSource>();
            ScopedDataSources = new List<IQDataSource>();
        }

        public QueryContext QueryContext;

        /// <summary>
        /// Область видимости имен данных
        /// </summary>
        public Dictionary<string, IQDataSource> Scope { get; set; }

        public List<IQDataSource> ScopedDataSources { get; set; }
    }
}