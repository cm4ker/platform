using Aquila.Core.Querying.Model;
using Aquila.QueryBuilder.Model;
using Aquila.QueryBuilder.Visitor;

namespace Aquila.Core.Querying
{
    public static class QLangHelper
    {
        public static void PrepareFromTop(this QLang machine)
        {
            var nameWalker = new PhysicalNameWalker();
            nameWalker.Visit(machine.top() as QItem);
        }

        
        public static string Compile(this QLang machine, bool popArg = false)
        {
            machine.PrepareFromTop();

            var realWalker = new RealWalker(machine.TypeManager);
            realWalker.Visit(machine.top() as QItem);

            if (popArg)
                machine.pop();

            var syntax = (realWalker.QueryMachine.pop() as SSyntaxNode);
            return new SQLVisitorBase().Visit(syntax);
        }
    }
}