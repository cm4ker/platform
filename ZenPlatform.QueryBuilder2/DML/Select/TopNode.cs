using System;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class TopNode : SqlNode
    {
        public TopNode(int count)
        {
            Count = count;
//            Childs.Add(Tokens.TopToken);
//            Childs.Add(Tokens.SpaceToken);
//            Childs.Add(new RawSqlNode(count.ToString()));
//            Childs.Add(Tokens.SpaceToken);
        }


        public int Count { get; set; }
    }
}