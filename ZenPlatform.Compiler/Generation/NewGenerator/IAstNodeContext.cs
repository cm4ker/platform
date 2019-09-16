using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;

namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    public interface IAstNodeContext
    {
        IAssemblyBuilder Assembly { get; set; }
        ITypeBuilder Type { get; set; }
        IMethodBuilder Method { get; set; }
        IConstructorBuilder Constructor { get; set; }
        IEmitter Emitter { get; set; }

        ILabel ReturnLabel { get; set; }
        ILocal Result { get; set; }
        bool InTry { get; set; }

        SyntaxNode SyntaxNode { get; set; }

        SymbolTable SymbolTable { get; set; }

        CompilationMode Mode { get; set; }

        SystemTypeBindings Bindings { get; set; }

        bool IsClass { get; set; }

        IAstNodeContext Copy();


        List<INodeEmitter> PreNodeEmitter { get; }
    }

    class AstNodeContext : IAstNodeContext
    {
        public IAssemblyBuilder Assembly { get; set; }
        public ITypeBuilder Type { get; set; }
        public IMethodBuilder Method { get; set; }
        public IConstructorBuilder Constructor { get; set; }
        public IEmitter Emitter { get; set; }
        public ILabel ReturnLabel { get; set; }
        public ILocal Result { get; set; }
        public bool InTry { get; set; }
        public SyntaxNode SyntaxNode { get; set; }
        public SymbolTable SymbolTable { get; set; }
        public CompilationMode Mode { get; set; }
        public SystemTypeBindings Bindings { get; set; }
        public bool IsClass { get; set; }

        public IAstNodeContext Copy()
        {
            return (IAstNodeContext) MemberwiseClone();
        }

        public List<INodeEmitter> PreNodeEmitter { get; }
    }
}