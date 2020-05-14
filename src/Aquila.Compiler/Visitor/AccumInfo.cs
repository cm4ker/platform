using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Infrastructure;

namespace Aquila.Compiler.Visitor
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
                //   _table.AddClass(string.Join(".", _namespaceStack.ToArray()), bc, bc.BindingType);
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