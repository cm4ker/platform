using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DML.Select;

namespace ZenPlatform.QueryBuilder2.DDL.CreateTable
{
    public class Token : SqlNode
    {
        public Token(string name)
        {
            Childs.Add(new RawSqlNode(name));
        }
    }
}