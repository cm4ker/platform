using ZenPlatform.Core.Querying.Model;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.Core.Querying
{
    public static class QLangHelper
    {
        public static void PrepareFromTop(this Querying.Model.QLang machine)
        {
            var nameWalker = new PhysicalNameWalker();
            nameWalker.Visit(machine.top() as QItem);
        }

        public static string Compile(this Querying.Model.QLang machine, bool popArg = false)
        {
            machine.PrepareFromTop();

            var realWalker = new RealWalker();
            realWalker.Visit(machine.top() as QItem);

            if (popArg)
                machine.pop();

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return new SQLVisitorBase().Visit(syntax);
        }
    }
}