using System.Collections.Generic;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Единица компиляции
    /// </summary>
    public class CompilationUnit
    {
        public CompilationUnit()
        {
            TypeEntities = new List<TypeEntity>();
        }

        public List<TypeEntity> TypeEntities { get; }
    }
}