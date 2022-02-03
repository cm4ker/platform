using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;
using Aquila.Runtime;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Querying
{
    public static class QLangHelper
    {
        public static string Compile(this QLangElement logicalTree, DatabaseRuntimeContext drContext)
        {
            //Create aliases for tree
            var pwalker = new PhysicalNameWalker(drContext);
            pwalker.Visit(logicalTree);

            var realWalker = new SelectionRealWalker(drContext);
            realWalker.Visit(logicalTree);

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return new MsSqlBuilder().Visit(syntax);
        }

        public static (string sql, QQueryList logicalTree) Compile(AqContext context, string sql)
        {
            var drc = context.DataRuntimeContext;
            var md = drc.Metadata.GetMetadata();

            var sec = drc.Metadata.GetMetadata().GetSecPoliciesFromRoles(context.Roles).ToList();

            //TODO: restrict user
            var ust = new UserSecTable();
            ust.Init(sec, md);

            var logicalTree = QLang.Parse(sql, md) as QQueryList ??
                              throw new Exception("Query stack machine after parsing MUST return the QueryList");

            //Transform tree
            logicalTree = (QQueryList)(new SecurityVisitor(md, ust).Visit(logicalTree));

            return (logicalTree.Compile(drc), logicalTree);
        }
    }
}