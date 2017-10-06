using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryCompiler
{
    /// <summary>
    /// Take responseability for construct FROM clause
    /// On this level of abstraction you can set relation types and condition between 
    /// tables\queryes\views\functions
    /// </summary>
    public class DBFromClause : IToken, IDBTableDataSourceContainer
    {
        private IDBTableDataSorce _root;
        private List<DBJoinClause> _joins;


        public DBFromClause()
        {
            _joins = new List<DBJoinClause>();
            CompileExpression = $"{SQLTokens.FROM}\n\t{{Table}}\n\t{{Joins}}";
        }

        public void AddTable(IDBTableDataSorce tds)
        {
            _root = tds;
        }

        public DBJoinClause Join(IDBTableDataSorce tds, JoinType joinType = JoinType.Left)
        {
            var jc = new DBJoinClause(tds, joinType);
            _joins.Add(jc);
            return jc;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var joins = new StringBuilder();

            foreach (var join in _joins)
            {
                joins.AppendLine(join.Compile());
            }

            return StandartCompilers.SimpleCompiler(CompileExpression, new { Table = _root.Compile().Replace("\n", "\n\t"), Joins = joins.Replace("\n", "\n\t") });
        }

        public string CompileExpression { get; set; }

        public IDBTableDataSorce RootTalbe => _root;

        public List<IDBTableDataSorce> Tables
        {
            get { return _joins.Select(x => x.DbTableDataSorce).Union(new[] { _root }).ToList(); }
        }

        public IDBTableDataSorce GetTable(string alias)
        {
            throw new NotImplementedException();
        }
    }

    public class Extesions
    {

    }
}