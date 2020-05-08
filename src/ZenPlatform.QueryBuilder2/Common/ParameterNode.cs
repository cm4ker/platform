﻿using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder.DML.Where
{
    public class ParameterNode : SqlNode
    {
        public ParameterNode(string parameterName)
        {
            Childs.Add(new RawSqlNode(parameterName));
        }
    }
}