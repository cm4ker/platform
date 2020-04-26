using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Queries;
using IQueryable = ZenPlatform.QueryBuilder.Queries.IQueryable;

namespace ZenPlatform.QueryBuilder
{
    /// <summary>
    /// Container for queryes. 
    /// Batches has parameters and can be executed
    /// </summary>
    public class DBBatch : IDBToken
    {
        private List<IQueryable> _queryes;

        public DBBatch()
        {
            _queryes = new List<IQueryable>();
        }

        public void AddQuery(IQueryable query)
        {
            _queryes.Add(query);
        }

        public void AddBatch(DBBatch batch)
        {
            _queryes.AddRange(batch._queryes);
        }

        public List<DBParameter> Parameters => GetParameters();
        public List<IQueryable> Queryes => _queryes;

        private List<DBParameter> GetParameters()
        {
            var result = new List<DBParameter>();
            foreach (var query in _queryes)
            {
                if (query is IDataChangeQuery)
                    result.AddRange((query as IDataChangeQuery).Parameters);
            }
            return result;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            foreach (var query in _queryes)
            {
                sb.AppendLine("-------------------------------------------------------");
                sb.AppendLine(query.Compile());
                sb.AppendLine("-------------------------------------------------------");
            }

            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}
