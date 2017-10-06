using System.Collections.Generic;
using System.Text;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class DeleteQuery : DeleteQueryBase
    {

    }

    public class InsertQuery : InsertQueryBase
    {

    }

    public class QueryBatch : List<IQueryObject>, IQueryObject
    {
        public string Compile()
        {
            var sb = new StringBuilder();
            foreach (var item in this)
            {
                sb.AppendLine(item.Compile());
            }
            return sb.ToString();
        }

        public string Compile(CompileOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}