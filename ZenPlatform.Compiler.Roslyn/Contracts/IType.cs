// using System.Collections.Generic;
// using System.IO;
// using ZenPlatform.Compiler.Roslyn.Contracts;
//
// namespace ZenPlatform.Compiler.Roslyn
// {
//     public interface SreType
//     {
//         IModule Module { get; }
//
//         SreType BaseType { get; }
//
//         IReadOnlyCollection<SreMethod> Methods { get; }
//
//         IReadOnlyCollection<SreConstructor> Constructors { get; }
//
//         string Name { get; }
//
//         string Namespace { get; }
//
//         string FullName { get; }
//
//         bool IsPublic { get; }
//
//         bool IsAbstract { get; }
//
//         void DumpRef(TextWriter tw);
//     }
// }