using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Core;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Symbols;
using ArrayTypeSyntax = ZenPlatform.Language.Ast.Definitions.ArrayTypeSyntax;
using SystemTypeBindings = ZenPlatform.Compiler.Roslyn.SystemTypeBindings;
using TypeSyntax = ZenPlatform.Language.Ast.Definitions.TypeSyntax;

namespace ZenPlatform.Compiler
{
    public class SyntaxTreeMemberAccessProvider
    {
        private readonly CompilationUnitList _cu;
        private readonly Root _root;
        private readonly SystemTypeBindings _stb;
        private List<TypeEntity> _types;

        private Dictionary<string, List<SreProperty>> _props;
        private Dictionary<string, List<SreInvokableBase>> _methods;
        private SreTypeSystem _ts;

        public SyntaxTreeMemberAccessProvider(Root root, SreTypeSystem ts)
        {
            _cu = root.Units;
            _root = root;

            _ts = ts;
            _stb = ts.GetSystemBindings();
            _types = _cu.GetNodes<TypeEntity>().ToList();

            _props = new Dictionary<string, List<SreProperty>>();
            _methods = new Dictionary<string, List<SreInvokableBase>>();

            RegisterStatic();
        }

        public SreType GetClrType(TypeSyntax typeSyntax)
        {
            if (typeSyntax is SingleTypeSyntax stn)
            {
                return GetClrType(stn);
            }
            else if (typeSyntax is GenericTypeSyntax gts)
            {
                SreType[] args = new SreType[gts.Args.Count];

                for (int i = 0; i < gts.Args.Count; i++)
                {
                    args[i] = GetClrType(gts.Args[i]);
                }

                //TODO: return resolved type
            }
            else if (typeSyntax is PrimitiveTypeSyntax pts)
            {
                return GetPrimitiveType(pts);
            }

            else if (typeSyntax is ArrayTypeSyntax atn)
            {
                return GetClrType(atn.ElementType).MakeArrayType();
            }

            else if (typeSyntax is UnionTypeSyntax utn)
            {
                throw new NotImplementedException();
            }

            throw new Exception("Type not resolved");
        }

        public SreType GetClrType(SingleTypeSyntax type)
        {
            var result = TypeFinder.Apply(type, _root);

            if (result == null)
                throw new Exception("Type not found");

            return result.ClrType;
        }

        private SreType GetPrimitiveType(PrimitiveTypeSyntax pts)
        {
            return pts.Kind switch
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
                TypeNodeKind.Context => _ts.Resolve<PlatformContext>(),
                _ => throw new Exception($"This type is not primitive {pts.Kind}")
            };
        }

        public SreMethod GetMethod(TypeSyntax type, string name, SreType[] args)
        {
            if (type is SingleTypeSyntax sts)
            {
                var symbol = TypeFinder.Apply(sts, _root);
                var typeDef = symbol.Type;

                var funcDef = typeDef?.TypeBody.SymbolTable.Find<MethodSymbol>(name, SymbolScopeBySecurity.User);

                SreMethod m = funcDef?.SelectOverload(args).clrMethod;

                return m ?? throw new Exception($"Property {name} not found");
            }
            else if (type is PrimitiveTypeSyntax pts)
            {
                var pType = GetPrimitiveType(pts);

                return pType.FindMethod(name) ?? throw new Exception("Property not found");
            }

            throw new Exception("Unknown type");
        }

        public SreProperty GetProperty(TypeSyntax type, string name)
        {
            if (type is SingleTypeSyntax sts)
            {
                var symbol = TypeFinder.Apply(sts, _root);
                var typeDef = symbol.Type;

                var propDef = typeDef?.TypeBody.SymbolTable.Find<PropertySymbol>(name, SymbolScopeBySecurity.User);
                var p = propDef?.ClrProperty;

                if (p == null)
                    throw new Exception($"Property {type}.{name} not found");
                return p;
            }
            else if (type is PrimitiveTypeSyntax pts)
            {
                var pType = GetPrimitiveType(pts);

                return pType.FindProperty(name) ?? throw new Exception("Property not found");
            }

            throw new Exception("Unknown type");
        }

        public List<string> GetMethods(TypeSyntax type)
        {
            var typeDef = GetClrType(GetTypeName(type));
            return typeDef.TypeBody.SymbolTable.GetAll<MethodSymbol>(SymbolType.Method).Select(x => x.Name).ToList();
        }

        public void RegisterProperty(string fullTypeName, SreProperty prop)
        {
            if (!_props.ContainsKey(fullTypeName))
                _props[fullTypeName] = new List<SreProperty>();

            _props[fullTypeName].Add(prop);
        }

        public void RegisterMethod(string fullTypeName, SreInvokableBase method)
        {
            if (!_methods.ContainsKey(fullTypeName))
                _methods[fullTypeName] = new List<SreInvokableBase>();

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
                    TypeNodeKind.Context => _stb.TypeSystem.Resolve<PlatformContext>().FullName,
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

        private SreProperty GetCachedProperty(string typeName, string propName)
        {
            if (_props.ContainsKey(typeName))
                return _props[typeName].FirstOrDefault(x => x.Name == propName);

            return null;
        }

        private SreInvokableBase GetCachedMethod(string typeName, string methodName, string[] typeNameArgs = null)
        {
            if (_methods.ContainsKey(typeName))
                return _methods[typeName]
                    .FirstOrDefault(x => x.Name == methodName);

            return null;
        }

        private TypeEntity GetClrType(string name)
        {
            return _types.FirstOrDefault(x =>
                (!string.IsNullOrEmpty(x.GetNamespace()) ? x.GetNamespace() + "." : "") + x.Name == name);
        }

        private void RegisterStatic()
        {
            var context = _stb.TypeSystem.Resolve<PlatformContext>();
            RegisterProperty(context.FullName, context.FindProperty(nameof(PlatformContext.Session)));
            RegisterProperty(context.FullName, context.FindProperty(nameof(PlatformContext.UserName)));
        }
    }
}