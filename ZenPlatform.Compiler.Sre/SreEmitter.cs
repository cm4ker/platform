using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Compiler.Sre
{
    class SreEmitter : IEmitter
    {
        private readonly ILGenerator _ilg;
        public ITypeSystem TypeSystem { get; }

        public SreEmitter(SreTypeSystem system, ILGenerator ilg)
        {
            TypeSystem = system;
            _ilg = ilg;
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
}