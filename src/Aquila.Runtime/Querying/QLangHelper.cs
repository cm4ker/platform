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
        public static string Compile(this QLangElement logicalTree, DatabaseRuntimeContext drContext,
            List<SMSecPolicy> secPolicies)
        {
            //Create aliases for tree
            var pwalker = new PhysicalNameWalker(drContext);
            pwalker.Visit(logicalTree);

            var ust = new UserSecTable();
            ust.Init(secPolicies);

            var realWalker = new RealWalker(drContext, ust);
            realWalker.Visit(logicalTree);

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return new MsSqlBuilder().Visit(syntax);
        }

        public static (string sql, QQueryList logicalTree) Compile(AqContext context, string sql)
        {
            var drc = context.DataRuntimeContext;

            var sec = drc.Metadata.GetMetadata().GetSecPoliciesFromRoles(context.Roles).ToList();

            var logicalTree = QLang.Parse(sql, drc.Metadata.GetMetadata()) as QQueryList ??
                              throw new Exception("Query stack machine after parsing MUST return the QueryList");

            return (logicalTree.Compile(drc, sec), logicalTree);
        }
    }
}