using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;

namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    public class AstGenerator : AstVisitorBase
    {
        private Stack<IAstNodeContext> _context;
        private List<ITransform> _transforms;

        private IAstNodeContext Context => _context.Peek();

        public AstGenerator(GeneratorParameters parameters)
        {
            _context = new Stack<IAstNodeContext>();

            var c = new AstNodeContext();

            c.Assembly = parameters.Builder;
            c.Mode = parameters.Mode;
            c.Bindings = new SystemTypeBindings(parameters.Builder.TypeSystem);

            _context.Push(c);
        }

        public override void VisitRoot(Root obj)
        {
            Context.SymbolTable = obj.SymbolTable;
        }

        public override void VisitClass(Class obj)
        {
            var tb = Context.SymbolTable.FindCodeObject<ITypeBuilder>(obj);

            //Change context 

            Context.Type = tb;
            Context.SymbolTable = obj.TypeBody.SymbolTable;
        }

        public override void VisitFunction(Function obj)
        {
            var method = Context.SymbolTable.FindCodeObject<IMethodBuilder>(obj);

            if (method is null)
                Break();

            Context.Method = method;
            Context.Emitter = method.Generator;

            var emitter = Context.Emitter;
            if (!obj.Type.Type.Equals(Context.Bindings.Void))
                Context.Result = emitter.DefineLocal(obj.Type.Type);
            Context.ReturnLabel = emitter.DefineLabel();

            obj.InstructionsBody.Accept(this);

            emitter.MarkLabel(Context.ReturnLabel);

            if (Context.Result != null)
                emitter.LdLoc(Context.Result);

            emitter.Ret();

            Break();
        }

        public override void VisitInstructionsBody(InstructionsBodyNode obj)
        {
            Context.SymbolTable = obj.SymbolTable;
        }

        public override void VisitVariable(Variable obj)
        {
            Context.AstNode = obj;
            (new VariableEmitter()).Emit(Context);
            (new VariableMultitypeNodeEmitter()).Emit(Context);
        }

        public override void BeforeVisitNode(AstNode node)
        {
            _context.Push(Context.Copy());
            Context.AstNode = node;
        }

        public override void AfterVisitNode(AstNode node)
        {
            _context.Pop();
        }
    }
}