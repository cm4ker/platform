using System;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Compiler.Visitor
{
    public class AstScopeRegister : AstWalker<object>
    {
        public static void Apply(SyntaxNode node)
        {
            var p = new AstScopeRegister();
            p.Visit(node);
        }

        private AstScopeRegister()
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

            if (obj.Parent is Constructor c)
            {
                c.Block.SymbolTable.Add(obj);
            }
            else
            {
                throw new Exception("Invalid register parameter in scope");
            }

            return null;
        }

        public override object VisitName(Name obj)
        {
            var v = obj.FirstParent<IScoped>().SymbolTable.Find(obj.Value, SymbolType.Variable, obj.GetScope());

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
            var st = obj.FirstParent<IScoped>()?.SymbolTable;

            if (obj.SymbolTable == null)
                obj.SymbolTable = new SymbolTable(st);

            obj.SymbolTable.Clear();
            return base.VisitTypeBody(obj);
        }

        public override object VisitModule(Module obj)
        {
            var st = obj.FirstParent<IScoped>()?.SymbolTable;
            st?.Add(obj);

            return base.VisitModule(obj);
        }

        public override object VisitClass(Class obj)
        {
            var st = obj.FirstParent<IScoped>().SymbolTable;
            st.Add(obj);
            return base.VisitClass(obj);
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

        public override object VisitConstructor(Constructor obj)
        {
            if (obj.Parent is TypeBody te)
            {
                if (obj.Block.SymbolTable == null)
                    obj.Block.SymbolTable = new SymbolTable(te.SymbolTable);

                obj.Block.SymbolTable.Clear();
            }
            else
            {
                throw new Exception("Invalid register function in scope");
            }

            return base.VisitConstructor(obj);
        }

        public override object VisitProperty(Property obj)
        {
            var parent = obj.FirstParent<TypeBody>().SymbolTable;

            if (obj.Setter != null && obj.Setter.SymbolTable == null)
            {
                obj.Setter.SymbolTable = new SymbolTable(parent);
            }

            if (obj.Getter != null && obj.Getter.SymbolTable == null)
            {
                obj.Getter.SymbolTable = new SymbolTable(parent);
            }

            obj.Getter?.SymbolTable.Clear();
            obj.Setter?.SymbolTable.Clear();

            obj.Setter?.SymbolTable.Add(new Parameter(null, "value", obj.Type, PassMethod.ByValue));

            return base.VisitProperty(obj);
        }


        public override object VisitIf(If obj)
        {
            var parent = obj.FirstParent<IScoped>().SymbolTable;

            obj.IfBlock.SymbolTable = new SymbolTable(parent);

            if (obj.ElseBlock != null)
                obj.ElseBlock.SymbolTable = new SymbolTable(parent);

            return base.VisitIf(obj);
        }

        public override object VisitMatchAtom(MatchAtom obj)
        {
            var parent = obj.FirstParent<IScoped>().SymbolTable;
            obj.Block.SymbolTable = new SymbolTable(parent);

            return base.VisitMatchAtom(obj);
        }

        public override object VisitFor(For obj)
        {
            if (obj.Block.SymbolTable == null)
            {
                var parent = obj.FirstParent<Block>();
                if (parent != null)
                    obj.Block.SymbolTable = new SymbolTable(parent.SymbolTable);
            }

            return base.VisitFor(obj);
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

            return base.VisitTry(obj);
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