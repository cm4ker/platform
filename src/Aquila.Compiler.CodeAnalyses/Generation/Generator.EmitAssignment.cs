// using System;
// using System.Linq;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using Aquila.Compiler.Contracts;
// using Aquila.Compiler.Helpers;
// using Aquila.Compiler.Infrastructure;
// using Aquila.Compiler.Roslyn;
// using Aquila.Compiler.Roslyn.RoslynBackend;
// using Aquila.Core.Querying;
// using Aquila.Language.Ast;
// using Aquila.Language.Ast.Definitions;
// using Aquila.Language.Ast.Definitions.Expressions;
// using Aquila.Language.Ast.Definitions.Functions;
// using Aquila.Language.Ast.Infrastructure;
// using Aquila.Language.Ast.Symbols;
// using Assignment = Aquila.Language.Ast.Assignment;
// using Name = Aquila.Language.Ast.Name;
// using Parameter = Aquila.Language.Ast.Parameter;
// using PropertyLookupExpression = Aquila.Language.Ast.PropertyLookupExpression;
// using TypeSyntax = Aquila.Language.Ast.TypeSyntax;
//
//
// namespace Aquila.Compiler.Generation
// {
//     public partial class Generator
//     {
//         private void EmitAssignment(RBlockBuilder e, Assignment assignment, SymbolTable symbolTable)
//         {
//             if (assignment.Assignable is Name name)
//             {
//                 var variable = symbolTable.Find<VariableSymbol>(name.Value, name.GetScope());
//                 if (variable == null)
//                     Error("Assignment variable " + name + " unknown.");
//
//                 // Non-indexed assignment
//                 if (assignment.Index == null)
//                 {
//                     bool mtNode = ((ITypedNode) variable.SyntaxObject).Type is UnionTypeSyntax;
//
//                     if (variable.CompileObject is RoslynParameter pd)
//                     {
//                         Parameter p = variable.SyntaxObject as Parameter;
//                         if (p.PassMethod == PassMethod.ByReference)
//                             e.LdArg(pd);
//                     }
//
//                     else if (variable.CompileObject is IField fl)
//                     {
//                         EmitLoadThis(e);
//                     }
//                     else if (variable.CompileObject is IProperty p)
//                     {
//                         EmitLoadThis(e);
//                     }
//                     // else if (variable.CompileObject is ILocal l && mtNode)
//                     //     e.LdLocA(l);
//
//                     // Load value
//                     EmitExpression(e, assignment.Value, symbolTable);
//
//                     // if (_map.GetClrType(name.Type) == _bindings.Object)
//                     //     HandleBox(e, assignment.Value.Type);
//
//                     // Store
//                     if (variable.CompileObject is RLocal vd)
//                         e.StLoc(vd);
//                     else if (variable.CompileObject is RoslynField fd)
//                         e.StFld(fd);
//                     else if (variable.CompileObject is RoslynParameter ppd)
//                     {
//                         // Parameter p = variable.SyntaxObject as Parameter;
//                         // if (p.PassMethod == PassMethod.ByReference)
//                         //     e.StIndI4();
//                         // else
//                         e.StArg(ppd);
//                     }
//                 }
//                 else
//                 {
//                     // // Load array.
//                     // if (variable.CompileObject is ILocal vd)
//                     //     e.LdLoc(vd);
//                     // else if (variable.CompileObject is IField fd)
//                     //     e.LdSFld(fd);
//                     // else if (variable.CompileObject is IParameter pd)
//                     //     e.LdArg(pd.Sequence);
//                     // // Load index.
//                     // EmitExpression(e, assignment.Index, symbolTable);
//                     // // Load value.
//                     // EmitExpression(e, assignment.Value, symbolTable);
//                     // // Set
//                     // e.StElemI4();
//                 }
//             }
//
//             if (assignment.Assignable is PropertyLookupExpression ple)
//             {
//                 var n = ple.Lookup as Name;
//
//                 //load context
//                 EmitExpression(e, ple.Current, symbolTable);
//
//                 var expProp = _map.GetProperty(ple.Current.Type, n.Value);
//
//                 //load value
//                 EmitExpression(e, assignment.Value, symbolTable);
//
//                 // if (expProp.PropertyType == _bindings.Object)
//                 //     HandleBox(e, assignment.Value.Type);
//
//                 //Set value
//                 e.StProp(expProp);
//             }
//         }
//
//         // private void HandleBox(IEmitter e, TypeSyntax type)
//         // {
//         //     HandleBox(e, _map.GetClrType(type));
//         // }
//
//         // private void HandleBox(IEmitter e, IType currenType)
//         // {
//         //     if (currenType.IsValueType && !currenType.IsArray)
//         //         e.Box(currenType);
//         // }
//
//         private void EmitLoadThis(RBlockBuilder e)
//         {
//             e.LdArg_0();
//         }
//
//         private void EmitLoad(IEmitter e, object codeObject, bool stat, bool addr)
//         {
//             if (stat)
//                 if (!addr)
//                     if (codeObject is IField f)
//                         e.LdFld(f);
//                     else if (codeObject is ILocal l)
//                         e.LdLoc(l);
//                     else if (codeObject is IParameter p)
//                         e.LdArg(p);
//         }
//     }
// }