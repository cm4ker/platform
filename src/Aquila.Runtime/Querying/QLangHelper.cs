using System;
using System.Collections.Generic;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;
using Aquila.Runtime;

namespace Aquila.Core.Querying
{
    public static class QLangHelper
    {
        public static string Compile(this QLangElement logicalTree, DatabaseRuntimeContext drContext)
        {
            //Create aliases for tree
            var pwalker = new PhysicalNameWalker(drContext);
            pwalker.Visit(logicalTree);

            var realWalker = new RealWalker(drContext);
            realWalker.Visit(logicalTree);

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return new MsSqlBuilder().Visit(syntax);
        }

        public static (string sql, QQueryList logicalTree) Compile(AqContext context, string sql)
        {
            var drc = context.DataRuntimeContext;
            var logicalTree = QLang.Parse(sql, drc.Metadata.GetMetadata()) as QQueryList ??
                              throw new Exception("Query stack machine after parsing MUST return the QueryList");

            return (logicalTree.Compile(drc), logicalTree);
        }
    }
}