using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;
using System.Xml.Schema;
using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IDeferredEmitter
    {
        IEmitter Emitter { get; }

        ITypeSystem TypeSystem { get; }
        IDeferredEmitter Emit(OpCode code);
        IDeferredEmitter Emit(OpCode code, IField field);
        IDeferredEmitter Emit(OpCode code, IMethod method);
        IDeferredEmitter Emit(OpCode code, IConstructor ctor);
        IDeferredEmitter Emit(OpCode code, string arg);
        IDeferredEmitter Emit(OpCode code, int arg);
        IDeferredEmitter Emit(OpCode code, long arg);
        IDeferredEmitter Emit(OpCode code, IType type);
        IDeferredEmitter Emit(OpCode code, float arg);
        IDeferredEmitter Emit(OpCode code, double arg);
        IDeferredEmitter DefineLocal(IType type);
        IDeferredEmitter DefineLabel();

        IDeferredEmitter BeginExceptionBlock();
        IDeferredEmitter BeginCatchBlock(IType exceptionType);
        IDeferredEmitter ThrowException(IType exceptionType);
        IDeferredEmitter EndExceptionBlock();

        IDeferredEmitter MarkLabel(ILabel label);
        IDeferredEmitter Emit(OpCode code, ILabel label);
        IDeferredEmitter Emit(OpCode code, ILocal local);
        IDeferredEmitter Emit(OpCode code, IParameter parameter);

        bool InitLocals { get; set; }

        void InsertSequencePoint(IFileSource file, int line, int position);
        SymbolTable SymbolTable { get; }

        void Flush();
    }

    /// <summary>
    /// Отложенная запись инструкций
    /// </summary>
    public class DeferredEmitter : IDeferredEmitter
    {
        private interface IDeferredInstruction
        {
            void Apply(IEmitter emitter);
        }


        private abstract class DeferredInstruction<T>
        {
            public DeferredInstruction(T value, OpCode opCode)
            {
                Value = value;
                OpCode = opCode;
            }

            public OpCode OpCode { get; set; }
            public T Value { get; set; }
        }

        private class DIField : DeferredInstruction<IField>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode, Value);
            }

            public DIField(IField value, OpCode opCode) : base(value, opCode)
            {
            }
        }

        private class DIMethod : DeferredInstruction<IMethod>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode, Value);
            }

            public DIMethod(IMethod value, OpCode opCode) : base(value, opCode)
            {
            }
        }

        private class DICtor : DeferredInstruction<IConstructor>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode, Value);
            }

            public DICtor(IConstructor value, OpCode opCode) : base(value, opCode)
            {
            }
        }

        private class DIString : DeferredInstruction<string>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode, Value);
            }

            public DIString(string value, OpCode opCode) : base(value, opCode)
            {
            }
        }

        private class DIInt : DeferredInstruction<int>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode, Value);
            }

            public DIInt(int value, OpCode opCode) : base(value, opCode)
            {
            }
        }

        private class DILong : DeferredInstruction<long>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode, Value);
            }

            public DILong(long value, OpCode opCode) : base(value, opCode)
            {
            }
        }

        private class DIOpCode : DeferredInstruction<object>, IDeferredInstruction
        {
            public void Apply(IEmitter emitter)
            {
                emitter.Emit(OpCode);
            }

            public DIOpCode(OpCode opCode) : base(null, opCode)
            {
            }
        }

        private readonly IEmitter _emitter;
        private List<IDeferredInstruction> _instructions;


        public DeferredEmitter(IEmitter emitter)
        {
            _emitter = emitter;
        }

        public IEmitter Emitter => _emitter;
        public ITypeSystem TypeSystem => _emitter.TypeSystem;

        public IDeferredEmitter Emit(OpCode code)
        {
            _instructions.Add(new DIOpCode(code));
            return this;
        }

        public IDeferredEmitter Emit(OpCode code, IField field)
        {
            _instructions.Add(new DIField(field, code));
            return this;
        }

        public IDeferredEmitter Emit(OpCode code, IMethod method)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, IConstructor ctor)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, string arg)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, int arg)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, long arg)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, IType type)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, float arg)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, double arg)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter DefineLocal(IType type)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter DefineLabel()
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter BeginExceptionBlock()
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter BeginCatchBlock(IType exceptionType)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter ThrowException(IType exceptionType)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter EndExceptionBlock()
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter MarkLabel(ILabel label)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, ILabel label)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, ILocal local)
        {
            throw new System.NotImplementedException();
        }

        public IDeferredEmitter Emit(OpCode code, IParameter parameter)
        {
            throw new System.NotImplementedException();
        }

        public bool InitLocals { get; set; }

        public void InsertSequencePoint(IFileSource file, int line, int position)
        {
            throw new System.NotImplementedException();
        }

        public SymbolTable SymbolTable { get; }

        public void Flush()
        {
            throw new System.NotImplementedException();
        }
    }
}