using System;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Visitor
{
    public class AstSymbolVisitor : AstVisitorBase
    {
        public override void VisitExpression(Expression e)
        {
        }

        public override void VisitVariable(Variable obj)
        {
            var ibn = obj.GetParent<InstructionsBodyNode>();
            if (ibn != null)
            {
                ibn.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception($"Invalid register variable in scope {obj.Name}");
            }
        }

        public override void VisitParameter(Parameter obj)
        {
            if (obj.Parent is Function f)
            {
                f.InstructionsBody.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register parameter in scope");
            }
        }

        public override void VisitTypeBody(TypeBody obj)
        {
            if (obj.SymbolTable == null)
                obj.SymbolTable = new SymbolTable();

            obj.SymbolTable.Clear();
        }

        public override void VisitFunction(Function obj)
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
        }

        public override void VisitFor(For obj)
        {
            if (obj.InstructionsBody.SymbolTable == null)
            {
                var parent = obj.GetParent<InstructionsBodyNode>();
                if (parent != null)
                    obj.InstructionsBody.SymbolTable = new SymbolTable(parent.SymbolTable);
            }
        }

        public override void VisitType(TypeNode obj)
        {
            if (obj.Type is UnknownArrayType)
            {
                obj.SetType(obj.Type.ArrayElementType.MakeArrayType());
            }
        }

        public override void VisitTry(Try obj)
        {
            if (obj.TryBlock.SymbolTable == null)
            {
                var parent = obj.GetParent<InstructionsBodyNode>();

                if (obj.TryBlock != null)
                    obj.TryBlock.SymbolTable = new SymbolTable(parent.SymbolTable);
                if (obj.CatchBlock != null)
                    obj.CatchBlock.SymbolTable = new SymbolTable(parent.SymbolTable);
            }
        }
    }
}