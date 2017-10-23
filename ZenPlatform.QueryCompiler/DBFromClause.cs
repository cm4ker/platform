using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZenPlatform.QueryCompiler.Interfaces;
using ZenPlatform.QueryCompiler.Queries;

namespace ZenPlatform.QueryCompiler
{
    /// <summary>
    /// Take responseability for construct FROM clause
    /// On this level of abstraction you can set relation types and condition between 
    /// tables\queryes\views\functions
    /// </summary>
    public class DBFromClause : IDBTablesContainer
    {
        private IDBTableDataSource _root;
        private List<DBJoinClause> _joins;


        public DBFromClause()
        {
            _joins = new List<DBJoinClause>();
            CompileExpression = $"{SQLTokens.FROM}\n\t{{Table}}\n\t{{Joins}}";
        }

        public void AddTable(IDBTableDataSource tds)
        {
            _root = tds;
        }

        public DBJoinClause Join(IDBTableDataSource tds, JoinType joinType = JoinType.Left)
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
            var joins = new StringBuilder();

            foreach (var join in _joins)
            {
                joins.AppendLine(join.Compile());
            }

            if (_root is null) return "";

            return $"{SQLTokens.FROM}\n\t{_root.Compile().Replace("\n", "\n\t")}\n\t{joins.Replace("\n", "\n\t")}";
        }

        public string CompileExpression { get; set; }

        public IDBTableDataSource RootTalbe => _root;

        public List<IDBTableDataSource> Tables
        {
            get { return _joins.Select(x => x.DbTableDataSorce).Union(new[] { _root }).ToList(); }
        }

        public IDBTableDataSource GetTable(string alias)
        {
            throw new NotImplementedException();
        }
    }

    public class Extesions
    {

    }
}