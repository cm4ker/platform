﻿using ZenPlatform.QueryBuilder2.Common;
using ZenPlatform.QueryBuilder2.DDL.CreateTable;

namespace ZenPlatform.QueryBuilder2.DML.Select
{
    public class TopNode : SqlNode
    {
        public TopNode(int count)
        {
            Childs.Add(Tokens.TopToken);
            Childs.Add(Tokens.SpaceToken);
            Childs.Add(new RawSqlNode(count.ToString()));
            Childs.Add(Tokens.SpaceToken);
        }
    }
}