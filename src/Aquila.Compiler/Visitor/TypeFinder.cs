using System;
using System.Collections.Generic;
using System.Linq;
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


        public static TypeSymbol Apply([NotNull] TypeSyntax typeNode, [NotNull] SyntaxNode node)
        {
            var p = new TypeFinder(typeNode);
            return p.Visit(node);
        }

        public TypeFinder(TypeSyntax typeNode)
        {
            _queue = new Queue<SyntaxNode>();

            _bodyUsings = typeNode.FirstParent<TypeBody>().Usings;
            _cuUsings = typeNode.FirstParent<CompilationUnit>().Usings;
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