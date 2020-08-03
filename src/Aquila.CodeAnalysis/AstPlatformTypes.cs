// using Aquila.Compiler.Contracts;
// using Aquila.Core;
// using Aquila.Language.Ast;
// using Aquila.Language.Ast.Definitions;
// using CompilationUnit = Aquila.Language.Ast.Definitions.CompilationUnit;
//
// namespace Aquila.Compiler
// {
//     public static class AstPlatformTypes
//     {
//         public static void System(Root root, ITypeSystem ts)
//         {
//             var sb = ts.GetSystemBindings();
//
//             var cu = new CompilationUnit(null, new UsingList(), new EntityList(), new NamespaceDeclarationList());
//             var ns = new NamespaceDeclaration(null, "System", null, new EntityList(), null);
//
//             cu.NamespaceDeclarations.Add(ns);
//
//             // cu.Entityes.Add(new BindingClass("String", sb.String));
//             // cu.Entityes.Add(new BindingClass("Int32", sb.Int));
//             // cu.Entityes.Add(new BindingClass("Int64", sb.Int64));
//             // cu.Entityes.Add(new BindingClass("Char", sb.Char));
//             // cu.Entityes.Add(new BindingClass("Double", sb.Double));
//             // cu.Entityes.Add(new BindingClass("Boolean", sb.Boolean));
//             // cu.Entityes.Add(new BindingClass("Byte", sb.Byte));
//             // cu.Entityes.Add(new BindingClass("DateTime", sb.DateTime));
//             // cu.Entityes.Add(new BindingClass("PlatformContext", ts.FindType<PlatformContext>()));
//         }
//     }
// }