using System;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using Class = ZenPlatform.Language.Ast.Class;
using Field = ZenPlatform.Language.Ast.Field;
using Function = ZenPlatform.Language.Ast.Function;
using Module = ZenPlatform.Language.Ast.Module;
using Property = ZenPlatform.Language.Ast.Property;
using Root = ZenPlatform.Language.Ast.Root;
using TypeBody = ZenPlatform.Language.Ast.TypeBody;
using Variable = ZenPlatform.Language.Ast.Variable;

namespace ZenPlatform.Compiler.Visitor
{
    public class AstSymbolPreparator : AstVisitorBase<object>
    {
        public static void Prepare(SyntaxNode node)
        {
            var p = new AstSymbolPreparator();
            p.Visit(node);
        }


        public override object DefaultVisit(SyntaxNode node)
        {
            foreach (var child in node.Children)
            {
                Visit(child);
            }

            return base.DefaultVisit(node);
        }

        private AstSymbolPreparator()
        {
        }

        public override object VisitVariable(Variable obj)
        {
            var ibn = obj.FirstParent<IScoped>();
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
                f.Block.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register parameter in scope");
            }

            return null;
        }

        public override object VisitName(Name obj)
        {
            var v = obj.FirstParent<IScoped>().SymbolTable.Find(obj.Value, SymbolType.Variable);

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
            var st = obj.FirstParent<IScoped>().SymbolTable;

            if (obj.SymbolTable == null)
                obj.SymbolTable = new SymbolTable(st);

            obj.SymbolTable.Clear();
            return null;
        }

        public override object VisitModule(Module obj)
        {
            var st = obj.FirstParent<IScoped>().SymbolTable;
            st.Add(obj);
            return null;
        }

        public override object VisitClass(Class obj)
        {
            var st = obj.FirstParent<IScoped>().SymbolTable;
            st.Add(obj);
            return null;
        }

        public override object VisitFunction(Function obj)
        {
            if (obj.Parent is TypeBody te)
            {
                if (obj.Block.SymbolTable == null)
                    obj.Block.SymbolTable = new SymbolTable(te.SymbolTable);

                obj.Block.SymbolTable.Clear();

                te.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register function in scope");
            }

            return base.VisitFunction(obj);
        }

        public override object VisitProperty(Property obj)
        {
            if (obj.Setter == null) return null;
            if (obj.Setter.SymbolTable == null)
            {
                var parent = obj.FirstParent<TypeBody>().SymbolTable;
                obj.Setter.SymbolTable = new SymbolTable(parent);
            }

            obj.Setter.SymbolTable.Add(new Parameter(null, "value", obj.Type, PassMethod.ByValue));
            return null;
        }

        public override object VisitFor(For obj)
        {
            if (obj.Block.SymbolTable == null)
            {
                var parent = obj.FirstParent<Block>();
                if (parent != null)
                    obj.Block.SymbolTable = new SymbolTable(parent.SymbolTable);
            }

            return null;
        }

        public override object VisitTry(Try obj)
        {
            if (obj.TryBlock.SymbolTable == null)
            {
                var parent = obj.FirstParent<Block>();

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

            return base.VisitRoot(root);
        }
    }
}