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
        public static void PrepareFromTop(this QLang machine,
            DatabaseRuntimeContext context)
        {
            var nameWalker = new PhysicalNameWalker(context);
            nameWalker.Visit(machine.top() as QLangElement);
        }


        public static string Compile(this QLang machine, EntityMetadataCollection col,
            DatabaseRuntimeContext context, bool popArg = false)
        {
            machine.PrepareFromTop(context);

            var realWalker = new RealWalker(context);
            realWalker.Visit(machine.top() as QLangElement);

            if (popArg)
                machine.pop();

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return new MsSqlBuilder().Visit(syntax);
        }
    }
}