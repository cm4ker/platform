using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class AliasNode : SqlNode
    {
        //        public string Alias { get; }

        public AliasNode(string alias)
        {
            Childs.Add(new IdentifierNode(alias));
            //Alias = alias;
        }
    }

    public class CaseNode : SqlNode
    {
        public CaseNode()
        {

        }

        public SqlNode WhenArg1 { get; private set; }
        public SqlNode WhenArg2 { get; private set; }
        public SqlNode WhenOp { get; private set; }

        public SqlNode ThenArg { get; set; }
        public SqlNode ElseArg { get; set; }

        CaseNode WithWhen(string arg1, string op, string arg2)
        {
            WhenArg1 = new RawSqlNode(arg1);
            WhenArg2 = new RawSqlNode(arg2);

            WhenOp = new RawSqlNode(op);
            return this;
        }

        CaseNode WithThen(string arg)
        {
            ThenArg = new RawSqlNode(arg);
            return this;
        }

    }
}