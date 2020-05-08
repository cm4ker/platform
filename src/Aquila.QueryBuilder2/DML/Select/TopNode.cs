using System;
using Aquila.QueryBuilder.Common;
using Aquila.Shared.Tree;

namespace Aquila.QueryBuilder.DML.Select
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