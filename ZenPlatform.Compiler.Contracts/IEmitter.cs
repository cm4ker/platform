using System.Reflection.Emit;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IEmitter
    {
        ITypeSystem TypeSystem { get; }
        IEmitter Emit(OpCode code);
        IEmitter Emit(OpCode code, IField field);
        IEmitter Emit(OpCode code, IMethod method);
        IEmitter Emit(OpCode code, IConstructor ctor);
        IEmitter Emit(OpCode code, string arg);
        IEmitter Emit(OpCode code, int arg);
        IEmitter Emit(OpCode code, long arg);
        IEmitter Emit(OpCode code, IType type);
        IEmitter Emit(OpCode code, float arg);
        IEmitter Emit(OpCode code, double arg);
        ILocal DefineLocal(IType type);
        ILabel DefineLabel();

        ILabel BeginExceptionBlock();
        IEmitter BeginCatchBlock(IType exceptionType);
        IEmitter ThrowException(IType exceptionType);
        IEmitter EndExceptionBlock();

        IEmitter MarkLabel(ILabel label);
        IEmitter Emit(OpCode code, ILabel label);
        IEmitter Emit(OpCode code, ILocal local);
        IEmitter Emit(OpCode code, IParameter parameter);

        bool InitLocals { get; set; }

        void InsertSequencePoint(IFileSource file, int line, int position);
        SymbolTable SymbolTable { get; }
    }
}