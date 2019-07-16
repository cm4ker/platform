using System;
using System.Xml;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    public interface INodeEmitter
    {
        bool Emit(IAstNodeContext context);
    }

    public class VariableEmitter : NodeEmitterBase
    {
        public bool Emit(IAstNodeContext context)
        {
            if (!Check<Variable>(context, (a) => !(a.Type is UnionTypeNode), out var variable)) return false;

            var e = context.Emitter;

            ILocal local = e.DefineLocal(null);

            context.SymbolTable.ConnectCodeObject(variable, local);

            if (variable.Value is Expression ex)
            {
                if (variable.Value != null)
                {
                    e.StLoc(local);
                }
            }

            return true;
        }
    }

    public class VariableMultitypeNodeEmitter : INodeEmitter
    {
        public bool Emit(IAstNodeContext context)
        {
            if (!(context.AstNode is Variable variable && variable.Type is UnionTypeNode mtn)) return false;

            var e = context.Emitter;

            var bindings = context.Bindings;

            ILocal local = e.DefineLocal(context.Bindings.UnionTypeStorage);
            var mt = context.Assembly.FindType("PlatformCustom.DefinedMultitypes");

            e.LdLocA(local)
                .LdsFld(mt.FindField(mtn.DeclName));

            context.SymbolTable.ConnectCodeObject(variable, local);

            if (variable.Value is null)
            {
                e.LdNull();
            }

            e.EmitCall(context.Bindings.UnionTypeStorage.FindConstructor(bindings.MultiType,
                bindings.Object));

            return true;
        }
    }

    public class LoaderEmitter : NodeEmitterBase
    {
        public override bool Emit(IAstNodeContext context)
        {
            var st = context.SymbolTable;
            var il = context.Emitter;

            if (Check<Assignment>(context, out var asg))
            {
                ISymbol variable = st.Find(asg.Name, SymbolType.Variable);

                //Preload context
                if (asg.Index == null)
                {
                    //not array
                    if (variable.CodeObject is IParameter p && variable.SyntaxObject is Parameter ps
                                                            && ps.PassMethod == PassMethod.ByReference)
                        il.LdArg(p.ArgIndex);
                    if (variable.CodeObject is IField)
                        il.LdArg_0();
                }
                else
                {
                    LoadVariable(variable, il);
                }
            }
            else if (Check<CallStatement>(context, out var call))
            {
                if (call.Arguments != null)
                {
                    foreach (var arg in call.Arguments)
                    {
                        // Regular value
                        if (arg.Value is Name n)
                        {
                            var variable = st.Find(n.Value, SymbolType.Variable);
                            LoadVariableReference(variable, il);
                        }
                        else if (arg.Value is IndexerExpression ue)
                        {
                            var variable = st.Find(((Name) ue.Value).Value, SymbolType.Variable);
                            LoadVariable(variable, il);
                        }
                        else
                        {
                        }
                    }
                }
            }

            return false;
        }

        private void LoadVariable(ISymbol variable, IEmitter il)
        {
            if (variable.CodeObject is ILocal vd)
                il.LdLoc(vd);
            else if (variable.CodeObject is IField fd)
            {
                //Load this and after that load field
                il.LdArg_0().LdFld(fd);
            }
            else if (variable.CodeObject is IParameter pd)
                il.LdArg(pd);
        }

        private void LoadVariableReference(ISymbol variable, IEmitter il)
        {
            if (variable.CodeObject is ILocal vd)
                il.LdLocA(vd);
            else if (variable.CodeObject is IField fd)
            {
                //Load this and after that load field
                il.LdArg_0().LdFldA(fd);
            }
            else if (variable.CodeObject is IParameter pd)
                il.LdArgA(pd);
        }
    }

    public class AssigmentEmitter : NodeEmitterBase
    {
        public bool Emit(IAstNodeContext context)
        {
            if (!Check<Assignment>(context, out var assignment)) return false;

            var symbolTable = context.SymbolTable;

            ISymbol variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            var il = context.Emitter;


            // Non-indexed assignment
            if (assignment.Index == null)
            {
                // Store value
                if (variable.CodeObject is ILocal vd)
                    il.StLoc(vd);
                else if (variable.CodeObject is IField fd)
                    il.StFld(fd);
                else if (variable.CodeObject is IParameter ppd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.StIndI4();
                    else
                        il.StArg(ppd);
                }
            }
            else
            {
                il.StElemI4();
            }

            return true;
        }
    }

    public class ReturnEmitter : NodeEmitterBase
    {
        public override bool Emit(IAstNodeContext context)
        {
            if (!Check<Return>(context)) return false;

            var e = context.Emitter;


            if (context.InTry)
            {
                e.StLoc(context.Result);
                e.Leave(context.ReturnLabel);
            }
            else
            {
                e.StLoc(context.Result)
                    .Br(context.ReturnLabel);
            }

            return true;
        }
    }

    public class CallEmitter : NodeEmitterBase
    {
        public override bool Emit(IAstNodeContext context)
        {
            if (!Check<CallStatement>(context, out var call)) return false;

            var st = context.SymbolTable;

            var symbol = st.Find(call.Name, SymbolType.Function);

            if (symbol is null) throw new NullReferenceException();

            var il = context.Emitter;

            if (symbol.SyntaxObject is Function function)
            {
                //
                // Check arguments
                //
                if (call.Arguments == null && function.Parameters == null)
                {
                    il.EmitCall((IMethod) symbol.CodeObject);
                }

                if (call.Arguments?.Count != function.Parameters?.Count)
                {
                    //Error("Argument mismatch [" + call.Name + "]");
                }
                else
                {
                    if (call.Arguments != null && function.Parameters != null)

                        for (int x = 0; x < call.Arguments.Count; x++)
                        {
                            if (call.Arguments[x].PassMethod != function.Parameters[x].PassMethod)
                            {
                                //Error("Argument error [" + call.Name + "], argument [" + x + "] is wrong.");
                            }
                        }
                }
            }

            if ((symbol.CodeObject as IMethod)?.ReturnType != context.Bindings.Void)
                il.Pop();

            return true;
        }
    }

    public abstract class NodeEmitterBase : INodeEmitter
    {
        public virtual bool Emit(IAstNodeContext context)
        {
            throw new NotImplementedException();
        }


        public bool Check<T>(IAstNodeContext context, Func<T, bool> criteria, out T node)
        {
            if (context.AstNode is T n)
            {
                node = n;
                return criteria(n);
            }

            node = default;

            return false;
        }

        public bool Check<T>(IAstNodeContext context, out T node)
        {
            return Check<T>(context, arg => true, out node);
        }

        public bool Check<T>(IAstNodeContext context, Func<T, bool> criteria)
        {
            return context.AstNode is T n && criteria(n);
        }

        public bool Check<T>(IAstNodeContext context)
        {
            return Check<T>(context, arg => true);
        }
    }
}