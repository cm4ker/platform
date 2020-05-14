namespace Aquila.Compiler.Roslyn
{
    public static class BlockExtensions
    {
        public static RBlockBuilder Inc(this RBlockBuilder block, RLocal loc)
        {
            return block
                .LdLoc(loc)
                .LdLit(1)
                .Add()
                .StLoc(loc);
        }
    }
}