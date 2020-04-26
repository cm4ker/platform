using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.QueryBuilder
{
    /// <summary>
    /// Take responseability for construct FROM clause
    /// On this level of abstraction you can set relation types and condition between 
    /// tables\queryes\views\functions
    /// </summary>
    public class DBFromClause : IDBTablesContainer
    {
        private IDBDataSource _root;
        private List<DBJoinClause> _joins;


        public DBFromClause()
        {
            _joins = new List<DBJoinClause>();
            CompileExpression = $"{SQLTokens.FROM}\n\t{{Table}}\n\t{{Joins}}";
        }

        public void AddTable(IDBDataSource tds)
        {
            _root = tds;
        }

        public DBJoinClause Join(IDBDataSource tds, JoinType joinType = JoinType.Left)
        {
            var jc = new DBJoinClause(tds, joinType);
            _joins.Add(jc);
            return jc;
        }



        public DBParameterCollection GetParameters()
        {
            var result = new DBParameterCollection();
            if (_root is IParametrized)
            {
                result.AddRange((_root as IParametrized).Parameters);
            }

            foreach (var join in _joins)
            {
                if (join is IParametrized)
                {
                    result.AddRange((_root as IParametrized).Parameters);
                }
            }
            return result;
        }



        public object Clone()
        {
            var clone = this.MemberwiseClone() as DBFromClause;
            clone._joins = _joins.ToList();

            return clone;
        }

        public string Compile(bool recompile = false)
        {
            if (_root is null) return "";

            var sb = new StringBuilder();

            sb.AppendLine(SQLTokens.FROM);

            sb.Append($"\t{_root.Compile().Replace("\n", "\n\t")}\n");

            foreach (var join in _joins)
            {
                sb.AppendLine(join.Compile().Replace("\n", "\n\t"));
            }

            return sb.ToString().Trim('\n');
        }

        public string CompileExpression { get; set; }

        public IDBDataSource RootTable => _root;

        public List<IDBDataSource> Tables
        {
            get { return _joins.Select(x => x.DbTableDataSorce).Union(new[] { _root }).ToList(); }
        }

        public IDBDataSource GetTable(string alias)
        {
            throw new NotImplementedException();
        }
    }

    public class Extesions
    {

    }
}