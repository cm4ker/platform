
using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Generation
{
    public class GeneratorParameters
    {
        public List<CompilationUnit> Units;
        public IAssemblyBuilder Builder;
        public CompilationMode Mode;

        public GeneratorParameters(List<CompilationUnit> units, IAssemblyBuilder builder, CompilationMode mode)
        {
            Units = units;
            Builder = builder;
            Mode = mode;
        }
    }
}