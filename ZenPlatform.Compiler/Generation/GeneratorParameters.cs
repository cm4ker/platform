using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Generation
{
    public class GeneratorParameters
    {
        public CompilationUnit Unit;
        public IAssemblyBuilder Builder;
        public CompilationMode Mode;

        public GeneratorParameters(CompilationUnit unit, IAssemblyBuilder builder, CompilationMode mode)
        {
            Unit = unit;
            Builder = builder;
            Mode = mode;
        }
    }
}