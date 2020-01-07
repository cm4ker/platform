using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Core;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ArrayTypeSyntax = ZenPlatform.Language.Ast.Definitions.ArrayTypeSyntax;
using TypeSyntax = ZenPlatform.Language.Ast.Definitions.TypeSyntax;

namespace ZenPlatform.Compiler
{
    public class SyntaxTreeMemberAccessProvider
    {
        private readonly List<CompilationUnit> _cu;
        private readonly SystemTypeBindings _stb;
        private List<TypeEntity> _types;

        private Dictionary<string, List<IProperty>> _props;
        private Dictionary<string, List<IMethod>> _methods;

        public SyntaxTreeMemberAccessProvider(List<CompilationUnit> cu, SystemTypeBindings stb)
        {
            _cu = cu;
            _stb = stb;
            _types = _cu.SelectMany(x => x.Entityes).ToList();

            _props = new Dictionary<string, List<IProperty>>();
            _methods = new Dictionary<string, List<IMethod>>();
        }

        public IMethod GetMethod(TypeSyntax type, string name, TypeSyntax[] args)
        {
            var typeName = GetTypeName(type);


            IMethod m = GetCachedMethod(typeName, name);

            if (m == null)
            {
                var typeDef = GetType(typeName);

                var funcDef = typeDef?.TypeBody.SymbolTable.Find(name, SymbolType.Method, SymbolScopeBySecurity.User);

                m = (IMethod) funcDef?.CodeObject;

                RegisterMethod(typeName, m);
            }

            return m;
        }

        public IProperty GetProperty(TypeSyntax type, string name)
        {
            var typeName = GetTypeName(type);
            IProperty p = GetCachedProperty(typeName, name);

            if (p == null)
            {
                var typeDef = GetType(typeName);
                var propDef = typeDef?.TypeBody.SymbolTable.Find(name, SymbolType.Property, SymbolScopeBySecurity.User);
                p = (IProperty) propDef?.CodeObject;
                RegisterProperty(typeName, p);
            }

            return p;
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
            return _types.FirstOrDefault(x => x.Name == name);
        }
    }
}