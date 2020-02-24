using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Avalonia.Remote.Protocol.Designer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Core;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ArrayTypeSyntax = ZenPlatform.Language.Ast.Definitions.ArrayTypeSyntax;
using TypeSyntax = ZenPlatform.Language.Ast.Definitions.TypeSyntax;

namespace ZenPlatform.Compiler
{
    public class SyntaxTreeMemberAccessProvider
    {
        private readonly CompilationUnitList _cu;
        private readonly Root _root;
        private readonly SystemTypeBindings _stb;
        private List<TypeEntity> _types;

        private Dictionary<string, List<IProperty>> _props;
        private Dictionary<string, List<IMethod>> _methods;
        private ITypeSystem _ts;

        public SyntaxTreeMemberAccessProvider(Root root, ITypeSystem ts)
        {
            _cu = root.Units;
            _root = root;

            _ts = ts;
            _stb = ts.GetSystemBindings();
            _types = _cu.GetNodes<TypeEntity>().ToList();

            _props = new Dictionary<string, List<IProperty>>();
            _methods = new Dictionary<string, List<IMethod>>();

            RegisterStatic();
        }

        public IType GetType(TypeSyntax typeSyntax)
        {
            if (typeSyntax is SingleTypeSyntax stn)
            {
                return GetType(stn);
            }
            else if (typeSyntax is GenericTypeSyntax gts)
            {
                IType[] args = new IType[gts.Args.Count];

                for (int i = 0; i < gts.Args.Count; i++)
                {
                    args[i] = GetType(gts.Args[i]);
                }

                //TODO: return resolved type
            }
            else if (typeSyntax is PrimitiveTypeSyntax ptn)
            {
                return ptn.Kind switch
                {
                    TypeNodeKind.Boolean => _stb.Boolean,
                    TypeNodeKind.Int => _stb.Int,
                    TypeNodeKind.Char => _stb.Char,
                    TypeNodeKind.Double => _stb.Double,
                    TypeNodeKind.String => _stb.String,
                    TypeNodeKind.Byte => _stb.Byte,
                    TypeNodeKind.Object => _stb.Object,
                    TypeNodeKind.Void => _stb.Void,
                    TypeNodeKind.Session => _stb.Session,
                    TypeNodeKind.Context => _ts.FindType<PlatformContext>(),
                    _ => throw new Exception($"This type is not primitive {ptn.Kind}")
                };
            }

            else if (typeSyntax is ArrayTypeSyntax atn)
            {
                return GetType(atn.ElementType).MakeArrayType();
            }

            else if (typeSyntax is UnionTypeSyntax utn)
            {
                throw new NotImplementedException();
            }

            throw new Exception("Type not resolved");
        }


        public IType GetType(SingleTypeSyntax type)
        {
            var result = TypeFinder.Apply(type, _root);

            if (result == null)
                throw new Exception("Type not found");

            return (IType) result.CodeObject;
        }

        public IMethod GetMethod(TypeSyntax type, string name, TypeSyntax[] args)
        {
            if (type is SingleTypeSyntax sts)
            {
                var symbol = TypeFinder.Apply(sts, _root);
                var typeDef = (TypeEntity) symbol.SyntaxObject;

                var funcDef = typeDef?.TypeBody.SymbolTable.Find(name, SymbolType.Method, SymbolScopeBySecurity.User);

                IMethod m = (IMethod) funcDef?.CodeObject;

                return m ?? throw new Exception($"Property {name} not found");
            }
            else if (type is PrimitiveTypeSyntax pts)
            {
                var pType = pts.Kind switch
                {
                    TypeNodeKind.Boolean => _stb.Boolean,
                    TypeNodeKind.Int => _stb.Int,
                    TypeNodeKind.Char => _stb.Char,
                    TypeNodeKind.Double => _stb.Double,
                    TypeNodeKind.String => _stb.String,
                    TypeNodeKind.Byte => _stb.Byte,
                    TypeNodeKind.Object => _stb.Object,
                    TypeNodeKind.Void => _stb.Void,
                    TypeNodeKind.Session => _stb.Session,
                    TypeNodeKind.Context => _ts.FindType<PlatformContext>(),
                    _ => throw new Exception($"This type is not primitive {pts.Kind}")
                };

                return pType.FindMethod(name) ?? throw new Exception("Property not found");
            }

            throw new Exception("Unknown type");
        }

        public IProperty GetProperty(TypeSyntax type, string name)
        {
            if (type is SingleTypeSyntax sts)
            {
                var symbol = TypeFinder.Apply(sts, _root);
                var typeDef = (TypeEntity) symbol.SyntaxObject;

                var propDef = typeDef?.TypeBody.SymbolTable.Find(name, SymbolType.Property, SymbolScopeBySecurity.User);
                var p = (IProperty) propDef?.CodeObject;

                if (p == null)
                    throw new Exception($"Property {type}.{name} not found");
                return p;
            }
            else if (type is PrimitiveTypeSyntax pts)
            {
                var pType = pts.Kind switch
                {
                    TypeNodeKind.Boolean => _stb.Boolean,
                    TypeNodeKind.Int => _stb.Int,
                    TypeNodeKind.Char => _stb.Char,
                    TypeNodeKind.Double => _stb.Double,
                    TypeNodeKind.String => _stb.String,
                    TypeNodeKind.Byte => _stb.Byte,
                    TypeNodeKind.Object => _stb.Object,
                    TypeNodeKind.Void => _stb.Void,
                    TypeNodeKind.Session => _stb.Session,
                    TypeNodeKind.Context => _ts.FindType<PlatformContext>(),
                    _ => throw new Exception($"This type is not primitive {pts.Kind}")
                };

                return pType.FindProperty(name) ?? throw new Exception("Property not found");
            }

            throw new Exception("Unknown type");
        }

        public List<string> GetMethods(TypeSyntax type)
        {
            var typeDef = GetType(GetTypeName(type));
            return typeDef.TypeBody.SymbolTable.GetAll(SymbolType.Method).Select(x => x.Name).ToList();
        }

        public void RegisterProperty(string fullTypeName, IProperty prop)
        {
            if (!_props.ContainsKey(fullTypeName))
                _props[fullTypeName] = new List<IProperty>();

            _props[fullTypeName].Add(prop);
        }

        public void RegisterMethod(string fullTypeName, IMethod method)
        {
            if (!_methods.ContainsKey(fullTypeName))
                _methods[fullTypeName] = new List<IMethod>();

            _methods[fullTypeName].Add(method);
        }

        private string GetTypeName(TypeSyntax type)
        {
            if (type is PrimitiveTypeSyntax pts)
            {
                return pts.Kind switch
                {
                    TypeNodeKind.String => _stb.String.FullName,
                    TypeNodeKind.Int => _stb.Int.FullName,
                    TypeNodeKind.Double => _stb.Double.FullName,
                    TypeNodeKind.Char => _stb.Char.FullName,
                    TypeNodeKind.Boolean => _stb.Boolean.FullName,
                    TypeNodeKind.Object => _stb.Object.FullName,
                    TypeNodeKind.Byte => _stb.Byte.FullName,
                    TypeNodeKind.Context => _stb.TypeSystem.FindType<PlatformContext>().FullName,
                    _ => throw new Exception("")
                };
            }
            else if (type is ArrayTypeSyntax)
            {
                throw new NotImplementedException();
            }
            else if (type is SingleTypeSyntax sts)
            {
                return sts.TypeName;
            }

            throw new NotImplementedException();
        }

        private IProperty GetCachedProperty(string typeName, string propName)
        {
            if (_props.ContainsKey(typeName))
                return _props[typeName].FirstOrDefault(x => x.Name == propName);

            return null;
        }

        private IMethod GetCachedMethod(string typeName, string methodName, string[] typeNameArgs = null)
        {
            if (_methods.ContainsKey(typeName))
                return _methods[typeName]
                    .FirstOrDefault(x => x.Name == methodName);

            return null;
        }

        private TypeEntity GetType(string name)
        {
            return _types.FirstOrDefault(x =>
                (!string.IsNullOrEmpty(x.GetNamespace()) ? x.GetNamespace() + "." : "") + x.Name == name);
        }

        private void RegisterStatic()
        {
            var context = _stb.TypeSystem.FindType<PlatformContext>();
            RegisterProperty(context.FullName, context.FindProperty(nameof(PlatformContext.Session)));
            RegisterProperty(context.FullName, context.FindProperty(nameof(PlatformContext.UserName)));
        }
    }
}