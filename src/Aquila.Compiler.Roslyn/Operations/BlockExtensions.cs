namespace Aquila.Compiler.Roslyn
{
    public static class BlockExtensions
    {
        public static RoslynEmitter Inc(this RoslynEmitter block, RLocal loc)
        {
            return block
                .LdLoc(loc)
                .LdLit(1)
                .Add()
                .StLoc(loc);
        }
    }
}