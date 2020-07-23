// using System.Collections.Generic;
// using Aquila.Compiler.Contracts;
// using Aquila.Compiler.Roslyn.RoslynBackend;
// using Aquila.Configuration.Structure;
// using Aquila.Core.Contracts;
// using Aquila.Language.Ast.Definitions;
// using Aquila.QueryBuilder;
//
// namespace Aquila.Compiler.Generation
// {
//     /// <summary>
//     /// Параметры генерации кода
//     /// </summary>
//     public class GeneratorParameters
//     {
//         /// <summary>
//         /// Для какой базы генерируется решение
//         /// </summary>
//         public SqlDatabaseType TargetDatabaseType { get; }
//
//         public IProject Configuration { get; }
//
//         public Root Root { get; }
//
//         // /// <summary>
//         // /// Юниты компиляции
//         // /// </summary>
//         // public CompilationUnitList Units { get; }
//
//         /// <summary>
//         /// Построитель сборки
//         /// </summary>
//         public RoslynAssemblyBuilder Builder { get; }
//
//         /// <summary>
//         /// Аттрибуты компиляции
//         /// </summary>
//         public CompilationMode Mode { get; }
//
//
//         public GeneratorParameters(Root root, RoslynAssemblyBuilder builder, CompilationMode mode,
//             SqlDatabaseType targetDatabaseType, IProject configuration)
//         {
//             TargetDatabaseType = targetDatabaseType;
//             Configuration = configuration;
//             Root = root;
//             Builder = builder;
//             Mode = mode;
//         }
//     }
// }