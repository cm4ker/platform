using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Compiler.Sre
{
    class SreEmitter : IEmitter
    {
        private readonly SreMethodEmitterProviderBase _provider;
        private readonly ILGenerator _ilg;
        public ITypeSystem TypeSystem { get; }

        public SreEmitter(SreTypeSystem system, SreMethodEmitterProviderBase provider)
        {
            _provider = provider;
            TypeSystem = system;
            _ilg = provider.Generator;
            SymbolTable = new SymbolTable();
        }

        public SreEmitter(SreTypeSystem system, ILGenerator ilg, SymbolTable parentTable)
        {
            TypeSystem = system;
            _ilg = ilg;
            SymbolTable = new SymbolTable(parentTable);
        }

        public IEmitter Emit(OpCode code)
        {
            _ilg.Emit(code);
            return this;
        }

        public IEmitter Emit(OpCode code, IMethod method)
        {
            _ilg.Emit(code, ((SreMethod) method).Method);
            return this;
        }

        public IEmitter Emit(OpCode code, IConstructor ctor)
        {
            _ilg.Emit(code, ((SreConstructor) ctor).Constuctor);
            return this;
        }

        public IEmitter Emit(OpCode code, string arg)
        {
            _ilg.Emit(code, arg);
            return this;
        }

        public IEmitter Emit(OpCode code, int arg)
        {
            _ilg.Emit(code, arg);
            return this;
        }

        public IEmitter Emit(OpCode code, long arg)
        {
            _ilg.Emit(code, arg);
            return this;
        }

        public IEmitter Emit(OpCode code, float arg)
        {
            _ilg.Emit(code, arg);
            return this;
        }

        public IEmitter Emit(OpCode code, double arg)
        {
            _ilg.Emit(code, arg);
            return this;
        }

        public ILocal DefineLocal(IType type)
        {
            return new SreLocal(_ilg.DeclareLocal(((SreType) type).Type));
        }

        public ILabel DefineLabel()
        {
            return new SreLabel(_ilg.DefineLabel());
        }

        public ILabel BeginExceptionBlock()
        {
            return new SreLabel(_ilg.BeginExceptionBlock());
        }

        public IEmitter BeginCatchBlock(IType exceptionType)
        {
            _ilg.BeginCatchBlock(((SreType) exceptionType).Type);
            return this;
        }

        public IEmitter EndExceptionBlock()
        {
            _ilg.EndExceptionBlock();
            return this;
        }

        public IEmitter ThrowException(IType exceptionType)
        {
            _ilg.ThrowException(((SreType) exceptionType).Type);
            return this;
        }

        public IEmitter MarkLabel(ILabel label)
        {
            _ilg.MarkLabel(((SreLabel) label).Label);
            return this;
        }

        public IEmitter Emit(OpCode code, ILabel label)
        {
            _ilg.Emit(code, ((SreLabel) label).Label);
            return this;
        }

        public IEmitter Emit(OpCode code, ILocal local)
        {
            _ilg.Emit(code, ((SreLocal) local).Local);
            return this;
        }

        public IEmitter Emit(OpCode code, IParameter parameter)
        {
            _ilg.Emit(code, ((SreParameter) parameter).ParameterInfo.Position);
            return this;
        }

        public bool InitLocals
        {
            get => _provider.InitLocals;
            set => _provider.InitLocals = value;
        }


        public void InsertSequencePoint(IFileSource file, int line, int position)
        {
        }

        public SymbolTable SymbolTable { get; }

        public IEmitter Emit(OpCode code, IType type)
        {
            _ilg.Emit(code, ((SreType) type).Type);
            return this;
        }


        public IEmitter Emit(OpCode code, IField field)
        {
            _ilg.Emit(code, ((SreField) field).Field);
            return this;
        }
    }


    abstract class SreMethodEmitterProviderBase
    {
        public abstract bool InitLocals { get; set; }

        public abstract ILGenerator Generator { get; }
    }


    class SreConstructorEmitterProvider : SreMethodEmitterProviderBase
    {
        private readonly ConstructorBuilder _cb;

        public SreConstructorEmitterProvider(ConstructorBuilder cb)
        {
            _cb = cb;
        }

        public override bool InitLocals
        {
            get => _cb.InitLocals;
            set => _cb.InitLocals = value;
        }

        public override ILGenerator Generator => _cb.GetILGenerator();
    }

    class SreMethodEmitterProvider : SreMethodEmitterProviderBase
    {
        private readonly MethodBuilder _mb;

        public SreMethodEmitterProvider(MethodBuilder mb)
        {
            _mb = mb;
        }

        public override bool InitLocals
        {
            get => _mb.InitLocals;
            set => _mb.InitLocals = value;
        }

        public override ILGenerator Generator => _mb.GetILGenerator();
    }
}