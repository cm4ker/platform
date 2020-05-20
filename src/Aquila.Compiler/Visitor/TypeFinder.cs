using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core;
using Aquila.Core.Annotations;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Compiler.Visitor
{
    public class TypeFinder : AstVisitorBase<TypeSymbol>
    {
        private readonly string _typeName;
        private string _ns;
        private UsingList _bodyUsings;
        private UsingList _cuUsings;
        private string _currentNamespace;
        private Queue<SyntaxNode> _queue;


        public static TypeSymbol FindSymbol([NotNull] TypeSyntax typeNode, [NotNull] SyntaxNode node)
        {
            if (typeNode.Parent == null)
                throw new Exception("Type has not parent. Can't create context without parent");

            var p = new TypeFinder(typeNode);
            return p.Visit(node);
        }

        public static RoslynType FindClr(TypeSyntax typeNode, RoslynTypeSystem ts)
        {
            if (typeNode is SingleTypeSyntax sts)
            {
                var contextNamespaces = typeNode.FirstParent<CompilationUnit>()?.Usings
                    .Union(typeNode.FirstParent<TypeBody>()?.Usings);

                var result = ts.FindType(sts.TypeName);

                if (result == null && contextNamespaces != null)
                {
                    foreach (var ns in contextNamespaces)
                    {
                        result = ts.FindType($"{ns.Name}.{sts.TypeName}");

                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            else if (typeNode is PrimitiveTypeSyntax pts)
            {
                var stb = ts.GetSystemBindings();

                return pts.Kind switch
                {
                    TypeNodeKind.Boolean => stb.Boolean,
                    TypeNodeKind.Int => stb.Int,
                    TypeNodeKind.Uid => stb.Guid,
                    TypeNodeKind.Char => stb.Char,
                    TypeNodeKind.Double => stb.Double,
                    TypeNodeKind.String => stb.String,
                    TypeNodeKind.Byte => stb.Byte,
                    TypeNodeKind.Object => stb.Object,
                    TypeNodeKind.Void => stb.Void,
                    TypeNodeKind.Session => stb.Session,
                    TypeNodeKind.Context => ts.Resolve<PlatformContext>(),
                    _ => throw new Exception($"This type is not primitive {pts.Kind}")
                };
            }


            return null;
        }

        public TypeFinder(TypeSyntax typeNode)
        {
            _queue = new Queue<SyntaxNode>();

            _bodyUsings = typeNode.FirstParent<TypeBody>()?.Usings;
            _cuUsings = typeNode.FirstParent<CompilationUnit>()?.Usings;
            _currentNamespace = typeNode.FirstParent<NamespaceDeclaration>()?.GetNamespace();

            var fullTypeName = GetTypeName(typeNode);

            if (fullTypeName.IndexOf('.') > 0)
            {
                _typeName = fullTypeName.Split('.').Last();
                _ns = string.Join('.', fullTypeName.Split('.')[..^1]);
            }
            else
            {
                _typeName = fullTypeName;
            }
        }

        private string GetTypeName(TypeSyntax type)
        {
            if (type is PrimitiveTypeSyntax pts)
            {
                return pts.Kind switch
                {
                    TypeNodeKind.String => "System.String",
                    TypeNodeKind.Int => "System.Int32",
                    TypeNodeKind.Double => "System.Double",
                    TypeNodeKind.Char => "System.Char",
                    TypeNodeKind.Boolean => "System.Boolean",
                    TypeNodeKind.Object => "System.Object",
                    TypeNodeKind.Byte => "System.Byte",
                    TypeNodeKind.Context => "System.PlatformContext",
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

        public override TypeSymbol VisitTypeBody(TypeBody arg)
        {
            return null;
        }

        public override TypeSymbol VisitClass(Class arg)
        {
            return null;
        }

        public override TypeSymbol VisitModule(Module arg)
        {
            return null;
        }

        public override TypeSymbol VisitNamespaceDeclaration(NamespaceDeclaration arg)
        {
            if (arg.GetNamespace() == _currentNamespace)
            {
                var typeSym = arg.SymbolTable.Find<TypeSymbol>(_typeName, SymbolScopeBySecurity.User);

                if (typeSym != null)
                    return typeSym;
            }

            return base.VisitNamespaceDeclaration(arg);
        }

        public override TypeSymbol DefaultVisit(SyntaxNode node)
        {
            _queue.Enqueue(node);

            var childs = node.Children.ToList();

            foreach (var child in childs)
            {
                var result = Visit(child as SyntaxNode);

                if (result != null)
                    return result;
            }

            return base.DefaultVisit(node);
        }
    }
}