using System;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using SqlPlusDbSync.QueryCompiler.Queryes.Operator;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public abstract class InsertQueryBase : IQueryObject
    {
        private Table _table;

        public Table Table
        {
            get { return _table; }
            set { _table = value; }
        }

        private string CompileInsertStatement()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO");
            sb.AppendFormat("\t{0}", _table.Alias);
            sb.AppendFormat("({0})", Table.GetFileds().Aggregate("", (s, ss) => { return $"{s},{(ss as Field).Name}"; }));
            sb.AppendLine();
            return sb.ToString();
        }


        private string CompileValuesStatement()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("VALUES");
            var i = 0;
            sb.Append("(");

            foreach (Field field in _table.GetFileds())
            {
                if (i > 0)
                    sb.Append(",");
                sb.Append($"'{{i++}}'");
            }
            sb.Append(")");
            sb.AppendLine();

            return sb.ToString();
        }



        public string Compile()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CompileInsertStatement());
            sb.Append(CompileValuesStatement());

            return sb.ToString();
        }

        public string Compile(CompileOptions options)
        {
            throw new NotImplementedException();
        }
    }
}