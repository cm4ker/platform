﻿using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Select
{
    public class SelectFieldNode : SqlNode
    {
        public SelectFieldNode(string fieldName)
        {
            Childs.Add(new IdentifierNode(fieldName));
        }

        public SelectFieldNode(string fieldName, string alias) : this(fieldName)
        {
            this.As(alias);
        }

        public SelectFieldNode(string tableName, string fieldName, string alias) : this(fieldName, alias)
        {
            this.WithTableName(tableName);
        }

        public SelectFieldNode As(string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                Childs.Add(new AliasNode(alias));

            return this;
        }

        public SelectFieldNode WithTableName(string tableName)
        {
            Childs.Insert(0, new IdentifierNode(tableName));
            Childs.Insert(1, Tokens.SchemaSeparator);

            return this;
        }
    }
}