using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Compiler.Generation
{
    /// <summary>
    /// Параметры генерации кода
    /// </summary>
    public class GeneratorParameters
    {
        /// <summary>
        /// Для какой базы генерируется решение
        /// </summary>
        public SqlDatabaseType TargetDatabaseType { get; }

        public IXCRoot Configuration { get; }

        /// <summary>
        /// Юниты компиляции
        /// </summary>
        public List<CompilationUnit> Units { get; }

        /// <summary>
        /// Построитель сборки
        /// </summary>
        public IAssemblyBuilder Builder { get; }

        /// <summary>
        /// Аттрибуты компиляции
        /// </summary>
        public CompilationMode Mode { get; }


        public GeneratorParameters(List<CompilationUnit> units, IAssemblyBuilder builder, CompilationMode mode,
            SqlDatabaseType targetDatabaseType, IXCRoot configuration)
        {
            TargetDatabaseType = targetDatabaseType;
            Configuration = configuration;
            Units = units;
            Builder = builder;
            Mode = mode;
        }
    }
}