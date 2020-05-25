using System;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public partial class RBlockBuilder : IEmitter
    {
        public IEmitter Emit(OpCode code)
        {
            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, IField field)
        {
            if (code == OpCodes.Stsfld)
                return StSFld(field);
            else if (code == OpCodes.Stfld)
                return StFld(field);

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, IMethod method)
        {
            var invokable = (RoslynInvokableBase) method;

            if (code == OpCodes.Call)
                return Call(invokable);
            else if (code == OpCodes.Calli)
                return Call(invokable);
            else if (code == OpCodes.Callvirt)
                return Call(invokable);

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, IConstructor ctor)
        {
            if (code == OpCodes.Newobj)
                return NewObj(ctor);

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, string arg)
        {
            return LdLit(arg);
        }

        public IEmitter Emit(OpCode code, int arg)
        {
            return LdLit(arg);
        }

        public IEmitter Emit(OpCode code, long arg)
        {
            return LdLit(arg);
        }

        public IEmitter Emit(OpCode code, IType type)
        {
            if (code == OpCodes.Newarr)
                return NewArr(type);

            if (code == OpCodes.Isinst)
                return IsInst(type);

            if (code == OpCodes.Castclass)
                return Cast(type);

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, float arg)
        {
            LdLit(arg);

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, double arg)
        {
            LdLit(arg);

            throw new Exception("Operation not supported");
        }

        public ILocal DefineLocal(IType type)
        {
            return DefineLocalInternal(type);
        }

        public ILabel DefineLabel()
        {
            throw new NotImplementedException();
        }

        public ILabel BeginExceptionBlock()
        {
        }

        public IEmitter BeginCatchBlock(IType exceptionType)
        {
        }

        public IEmitter EndExceptionBlock()
        {
        }

        public IEmitter MarkLabel(ILabel label)
        {
        }

        public IEmitter Emit(OpCode code, ILabel label)
        {
        }

        public IEmitter Emit(OpCode code, ILocal local)
        {
        }

        public IEmitter Emit(OpCode code, IParameter parameter)
        {
        }

        public bool InitLocals { get; set; }

        public void InsertSequencePoint(IFileSource file, int line, int position)
        {
        }
    }
}