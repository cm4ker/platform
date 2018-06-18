﻿using ZenPlatform.QueryBuilder2.Common;

namespace ZenPlatform.QueryBuilder2.DML.Select
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
}