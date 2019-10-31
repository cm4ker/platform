﻿using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Visitor
{
    public class PostgreVisitor : SQLQueryVisitorBase
    {

        public override string VisitTopNode(TopNode node)
        {
            return string.Format("limit {0}{1}",
                node.Limit,
                node.Offset == 0 ? "" : $", { node.Offset}");

        }


        public override string VisitSelectNode(SelectNode node)
        {
            return string.Format("SELECT {0}\n{1}{2}{3}{4}{5}",
                string.Join(",\n", node.Fields.Select(f => f.Accept(this))),
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this),
                node.GroupBy == null ? "" : node.GroupBy.Accept(this),
                node.OrderBy == null ? "" : node.OrderBy.Accept(this),
                node.Top == null ? "" : node.OrderBy.Accept(this)
            );

        }
    }
}
