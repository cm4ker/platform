using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.Language.Ast.Symbols;
using Module = ZenPlatform.Language.Ast.Definitions.Module;

namespace ZenPlatform.Compiler.Visitor
{
    public class LoweringOptimizer : AstWalker<object>
    {
        private readonly ITypeSystem _ts;

        public LoweringOptimizer(ITypeSystem ts)
        {
            _ts = ts;
        }

        public static void Apply(ITypeSystem ts, SyntaxNode node)
        {
            var p = new LoweringOptimizer(ts);
            p.Visit(node);
        }

        public override object VisitBinaryExpression(BinaryExpression obj)
        {
            base.VisitBinaryExpression(obj);

            if (obj.BinaryOperatorType == BinaryOperatorType.Add && obj.Left.Type.Kind == TypeNodeKind.String)
            {
                var left = new Argument(null, obj.Left);
                var right = new Argument(null, obj.Right);

                var call = new ClrInternalCall(_ts.GetSystemBindings().Methods.Concat,
                    new ArgumentList {left, right});

                obj.Parent.Replace(obj, call);
            }

            return null;
        }
    }

    public class TypeFinder : AstVisitorBase<TypeSymbol>
    {
        private readonly string _typeName;
        private string _ns;
        private UsingList _bodyUsings;
        private UsingList _cuUsings;
        private string _currentNamespace;
        private Queue<SyntaxNode> _queue;


        public static TypeSymbol Apply(TypeSyntax typeNode, SyntaxNode node)
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

            var childs = node.Childs.ToList();

            foreach (var child in childs)
            {
                var result = Visit(child as SyntaxNode);

                if (result != null)
                    return result;
            }

            return base.DefaultVisit(node);
        }
    }

    /// <summary>
    /// Визитор для заполнения классов
    /// </summary>
    public class AccumInfo : AstWalker<object>
    {
        private ClassTable _table;

        public List<string> _cuNamespace;
        public List<string> _namespaceNamespace;
        public List<string> _bodyNamespace;

        public Stack<string> _namespaceStack;

        public AccumInfo()
        {
            _cuNamespace = new List<string>();
            _bodyNamespace = new List<string>();
            _namespaceStack = new Stack<string>();
            _table = new ClassTable();
        }

        public ClassTable Table => _table;

        public override object VisitNamespaceDeclaration(NamespaceDeclaration obj)
        {
            _namespaceStack.Push(obj.Name);
            base.VisitNamespaceDeclaration(obj);
            _namespaceStack.Pop();

            return null;
        }

        public override object VisitCompilationUnit(CompilationUnit obj)
        {
            _cuNamespace.Clear();

            foreach (var @using in obj.Usings)
            {
                if (@using is UsingDeclaration ud)
                    _cuNamespace.Add(ud.Name);
            }

            return base.VisitCompilationUnit(obj);
        }


        public override object VisitClass(Class obj)
        {
            return base.VisitClass(obj);
        }


        public override object VisitTypeEntity(TypeEntity obj)
        {
            if (obj is BindingClass bc)
            {
                _table.AddClass(string.Join(".", _namespaceStack.ToArray()), bc, bc.BindingType);
            }

            return base.VisitTypeEntity(obj);
        }

        public override object VisitTypeBody(TypeBody obj)
        {
            _bodyNamespace.Clear();

            foreach (var @using in obj.Usings)
            {
                if (@using is UsingDeclaration ud)
                    _bodyNamespace.Add(ud.Name);
            }


            return base.VisitTypeBody(obj);
        }
    }
}