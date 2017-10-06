using System;
using System.Text;
using SqlPlusDbSync.QueryCompiler.Queryes.Operator;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public abstract class DeleteQueryBase : IQueryObject
    {
        private Table _table;

        public Table Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public IBooleanResultOperator WhereOperator { get; set; }


        private string CompileDeleteStatement()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DELETE");
            sb.AppendFormat("\t{0}", _table.Alias);
            sb.AppendLine();
            return sb.ToString();
        }

        private string CompileFromStatement()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("FROM");
            sb.AppendFormat("\t{0}", _table.Compile());
            sb.AppendLine();

            return sb.ToString();
        }

        public virtual string CompileWhereStatement()
        {
            if (WhereOperator is null) return "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("WHERE");
            sb.AppendFormat("\t{0}", WhereOperator.Compile());
            sb.AppendLine();
            return sb.ToString();
        }


        public string Compile()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CompileDeleteStatement());
            sb.Append(CompileFromStatement());
            sb.Append(CompileWhereStatement());


            return sb.ToString();
        }

        public string Compile(CompileOptions options)
        {
            throw new NotImplementedException();
        }
    }
}