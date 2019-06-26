using System;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Visitor
{
    public class AstSymbolVisitor : AstVisitorBase<object>
    {
        public override object VisitExpression(Expression e)
        {
            return null;
        }

        public override object VisitVariable(Variable obj)
        {
            var ibn = obj.GetParent<IScoped>();
            if (ibn != null)
            {
                ibn.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception($"Invalid register variable in scope {obj.Name}");
            }

            return null;
        }

        public override object VisitParameter(Parameter obj)
        {
            if (obj.Parent is Function f)
            {
                f.InstructionsBody.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register parameter in scope");
            }

            return null;
        }

        public override object VisitName(Name obj)
        {
            var v = obj.GetParent<IScoped>().SymbolTable.Find(obj.Value, SymbolType.Variable);

            if (v?.SyntaxObject is Variable vv) obj.Type = vv.Type;
            if (v?.SyntaxObject is Parameter p) obj.Type = p.Type;

            return null;
        }

        public override object VisitField(Field obj)
        {
            if (obj.Parent is TypeBody f)
            {
                f.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register field in scope");
            }

            return null;
        }

        public override object VisitTypeBody(TypeBody obj)
        {
            var st = obj.GetParent<IScoped>().SymbolTable;

            if (obj.SymbolTable == null)
                obj.SymbolTable = new SymbolTable(st);

            obj.SymbolTable.Clear();
            return null;
        }

        public override object VisitModule(Module obj)
        {
            var st = obj.GetParent<IScoped>().SymbolTable;
            st.Add(obj);
            return null;
        }

        public override object VisitClass(Class obj)
        {
            var st = obj.GetParent<IScoped>().SymbolTable;
            st.Add(obj);
            return null;
        }

        public override object VisitFunction(Function obj)
        {
            if (obj.Parent is TypeBody te)
            {
                if (obj.InstructionsBody.SymbolTable == null)
                    obj.InstructionsBody.SymbolTable = new SymbolTable(te.SymbolTable);

                obj.InstructionsBody.SymbolTable.Clear();

                te.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register function in scope");
            }

            return null;
        }

        public override object VisitProperty(Property obj)
        {
            if (obj.Setter == null) return null;
            if (obj.Setter.SymbolTable == null)
            {
                var parent = obj.GetParent<TypeBody>().SymbolTable;
                obj.Setter.SymbolTable = new SymbolTable(parent);
            }

            obj.Setter.SymbolTable.Add(new Parameter(null, "value", obj.Type, PassMethod.ByValue));
            return null;
        }

        public override object VisitFor(For obj)
        {
            if (obj.InstructionsBody.SymbolTable == null)
            {
                var parent = obj.GetParent<InstructionsBodyNode>();
                if (parent != null)
                    obj.InstructionsBody.SymbolTable = new SymbolTable(parent.SymbolTable);
            }

            return null;
        }

        public override object VisitSingleType(SingleTypeNode obj)
        {
            if (obj.Type is UnknownArrayType)
            {
                obj.SetType(obj.Type.ArrayElementType.MakeArrayType());
            }

            return null;
        }

        public override object VisitTry(Try obj)
        {
            if (obj.TryBlock.SymbolTable == null)
            {
                var parent = obj.GetParent<InstructionsBodyNode>();

                if (obj.TryBlock != null)
                    obj.TryBlock.SymbolTable = new SymbolTable(parent.SymbolTable);
                if (obj.CatchBlock != null)
                    obj.CatchBlock.SymbolTable = new SymbolTable(parent.SymbolTable);
            }

            return null;
        }

        public override object VisitRoot(Root root)
        {
            if (root.SymbolTable == null)
            {
                root.SymbolTable = new SymbolTable();
            }

            return null;
        }
    }
}