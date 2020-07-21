// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Aquila.Compiler.Contracts;
// using Aquila.Compiler.Roslyn;
// using Aquila.Compiler.Roslyn.RoslynBackend;
// using Aquila.Compiler.Visitor;
// using Aquila.Core;
// using Aquila.Language.Ast;
// using Aquila.Language.Ast.Definitions;
// using Aquila.Language.Ast.Definitions.Functions;
// using Aquila.Language.Ast.Misc;
// using Aquila.Language.Ast.Symbols;
// using ArrayTypeSyntax = Aquila.Language.Ast.ArrayTypeSyntax;
// using GenericTypeSyntax = Aquila.Language.Ast.GenericTypeSyntax;
// using PrimitiveTypeSyntax = Aquila.Language.Ast.PrimitiveTypeSyntax;
// using SingleTypeSyntax = Aquila.Language.Ast.SingleTypeSyntax;
// using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;
// using TypeSyntax = Aquila.Language.Ast.TypeSyntax;
//
// namespace Aquila.Compiler
// {
//     public class SyntaxTreeMemberAccessProvider
//     {
//         private readonly CompilationUnitList _cu;
//         private readonly Root _root;
//         private readonly SystemTypeBindings _stb;
//         private List<TypeEntity> _types;
//
//         private Dictionary<string, List<RoslynProperty>> _props;
//         private Dictionary<string, List<RoslynInvokableBase>> _methods;
//         private RoslynTypeSystem _ts;
//
//         public SyntaxTreeMemberAccessProvider(Root root, RoslynTypeSystem ts)
//         {
//             _cu = root.Units;
//             _root = root;
//
//             _ts = ts;
//             _stb = ts.GetSystemBindings();
//             _types = _cu.GetNodes<TypeEntity>().ToList();
//
//             _props = new Dictionary<string, List<RoslynProperty>>();
//             _methods = new Dictionary<string, List<RoslynInvokableBase>>();
//
//             RegisterStatic();
//         }
//
//         public RoslynType GetClrType(TypeSyntax typeSyntax)
//         {
//             if (typeSyntax is SingleTypeSyntax stn)
//             {
//                 return GetClrType(stn);
//             }
//             else if (typeSyntax is GenericTypeSyntax gts)
//             {
//                 RoslynType[] args = new RoslynType[gts.Args.Count];
//
//                 for (int i = 0; i < gts.Args.Count; i++)
//                 {
//                     args[i] = GetClrType(gts.Args[i]);
//                 }
//
//                 //TODO: return resolved type
//             }
//             else if (typeSyntax is PrimitiveTypeSyntax pts)
//             {
//                 return TypeFinder.FindClr(pts, _ts);
//             }
//
//             else if (typeSyntax is ArrayTypeSyntax atn)
//             {
//                 return GetClrType(atn.ElementType).MakeArrayType();
//             }
//
//             else if (typeSyntax is UnionTypeSyntax utn)
//             {
//                 throw new NotImplementedException();
//             }
//
//             throw new Exception("Type not resolved");
//         }
//
//         public RoslynType GetClrType(SingleTypeSyntax type)
//         {
//             var result = TypeFinder.FindSymbol(type, _root)?.ClrType;
//
//             if (result == null)
//             {
//                 result = TypeFinder.FindClr(type, _ts);
//
//                 if (result == null)
//                     throw new Exception($"Type not found {type.TypeName}");
//             }
//
//             return result;
//         }
//
//
//         public (RoslynMethod clrMethod, Method astMethod) GetMethod(TypeSyntax type, string name, RoslynType[] args)
//         {
//             if (type is SingleTypeSyntax sts)
//             {
//                 var symbol = TypeFinder.FindSymbol(sts, _root);
//                 var typeDef = symbol.Type;
//
//                 var funcDef = typeDef?.TypeBody.SymbolTable.Find<MethodSymbol>(name, SymbolScopeBySecurity.User);
//
//                 var a = funcDef?.SelectOverload(args) ?? throw new Exception($"Method {name} not found");
//
//                 RoslynMethod clr = a.clrMethod;
//                 Method ast = a.method;
//
//                 return (clr, ast);
//             }
//             else if (type is PrimitiveTypeSyntax pts)
//             {
//                 var pType = TypeFinder.FindClr(pts, _ts);
//                 var clrMethod = pType.FindMethod(name) ?? throw new Exception("Property not found");
//                 return (clrMethod, null);
//             }
//
//             throw new Exception("Unknown type");
//         }
//
//         public RoslynProperty GetProperty(TypeSyntax type, string name)
//         {
//             if (type is SingleTypeSyntax sts)
//             {
//                 var symbol = TypeFinder.FindSymbol(sts, _root);
//
//                 var typeDef = symbol.Type;
//
//                 var propDef = typeDef?.TypeBody.SymbolTable.Find<PropertySymbol>(name, SymbolScopeBySecurity.User);
//                 var p = propDef?.ClrProperty;
//
//                 if (p == null)
//                     throw new Exception($"Property {type}.{name} not found");
//                 return p;
//             }
//             else if (type is PrimitiveTypeSyntax pts)
//             {
//                 var pType = TypeFinder.FindClr(pts, _ts);
//
//                 return pType.FindProperty(name) ?? throw new Exception("Property not found");
//             }
//
//             throw new Exception("Unknown type");
//         }
//
//         public List<string> GetMethods(TypeSyntax type)
//         {
//             var typeDef = GetClrType(GetTypeName(type));
//             return typeDef.TypeBody.SymbolTable.GetAll<MethodSymbol>(SymbolType.Method).Select(x => x.Name).ToList();
//         }
//
//         public void RegisterProperty(string fullTypeName, RoslynProperty prop)
//         {
//             if (!_props.ContainsKey(fullTypeName))
//                 _props[fullTypeName] = new List<RoslynProperty>();
//
//             _props[fullTypeName].Add(prop);
//         }
//
//         public void RegisterMethod(string fullTypeName, RoslynInvokableBase method)
//         {
//             if (!_methods.ContainsKey(fullTypeName))
//                 _methods[fullTypeName] = new List<RoslynInvokableBase>();
//
//             _methods[fullTypeName].Add(method);
//         }
//
//         private string GetTypeName(TypeSyntax type)
//         {
//             if (type is PrimitiveTypeSyntax pts)
//             {
//                 return pts.Kind switch
//                 {
//                     TypeNodeKind.String => _stb.String.FullName,
//                     TypeNodeKind.Int => _stb.Int.FullName,
//                     TypeNodeKind.Double => _stb.Double.FullName,
//                     TypeNodeKind.Char => _stb.Char.FullName,
//                     TypeNodeKind.Boolean => _stb.Boolean.FullName,
//                     TypeNodeKind.Object => _stb.Object.FullName,
//                     TypeNodeKind.Byte => _stb.Byte.FullName,
//                     TypeNodeKind.Context => _stb.TypeSystem.Resolve<PlatformContext>().FullName,
//                     _ => throw new Exception("")
//                 };
//             }
//             else if (type is ArrayTypeSyntax)
//             {
//                 throw new NotImplementedException();
//             }
//             else if (type is SingleTypeSyntax sts)
//             {
//                 return sts.TypeName;
//             }
//
//             throw new NotImplementedException();
//         }
//
//         private RoslynProperty GetCachedProperty(string typeName, string propName)
//         {
//             if (_props.ContainsKey(typeName))
//                 return _props[typeName].FirstOrDefault(x => x.Name == propName);
//
//             return null;
//         }
//
//         private RoslynInvokableBase GetCachedMethod(string typeName, string methodName, string[] typeNameArgs = null)
//         {
//             if (_methods.ContainsKey(typeName))
//                 return _methods[typeName]
//                     .FirstOrDefault(x => x.Name == methodName);
//
//             return null;
//         }
//
//         private TypeEntity GetClrType(string name)
//         {
//             return _types.FirstOrDefault(x =>
//                 (!string.IsNullOrEmpty(x.GetNamespace()) ? x.GetNamespace() + "." : "") + x.Name == name);
//         }
//
//         private void RegisterStatic()
//         {
//             var context = _stb.TypeSystem.Resolve<PlatformContext>();
//             RegisterProperty(context.FullName, context.FindProperty(nameof(PlatformContext.Session)));
//             RegisterProperty(context.FullName, context.FindProperty(nameof(PlatformContext.UserName)));
//         }
//     }
// }