using System;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.Operations;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public partial class RoslynEmitter : IEmitter
    {
        public IEmitter Emit(OpCode code)
        {
            if (code == OpCodes.Ret)
                return Ret();

            if (code == OpCodes.Ldarg_0)
                return LdArg_0();
            if (code == OpCodes.Ldarg_1)
                return LdArg(1);
            if (code == OpCodes.Ldarg_2)
                return LdArg(2);
            if (code == OpCodes.Ldarg_3)
                return LdArg(3);

            throw new Exception("Operation not supported: " + code);
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
            return new RLabel(GetNextLabelIndex());
        }

        public ILabel BeginExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public IEmitter BeginCatchBlock(IType exceptionType)
        {
            throw new NotImplementedException();
        }

        public IEmitter EndExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public IEmitter MarkLabel(ILabel label)
        {
            return Mark((RLabel) label);
        }

        public IEmitter Emit(OpCode code, ILabel label)
        {
            if (code == OpCodes.Br)
            {
                Goto(label);
            }
            else if (code == OpCodes.Br_S)
            {
                Goto(label);
            }
            else if (code == OpCodes.Brfalse)
            {
                Neg().Block()
                    .Goto(label)
                    .EndBlock();
            }
            else if (code == OpCodes.Brtrue)
            {
                Neg().Block()
                    .Goto(label)
                    .EndBlock();
            }

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, ILocal local)
        {
            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, IParameter parameter)
        {
            if (code == OpCodes.Ldarg)
                return LdArg((RoslynParameter) parameter);

            throw new Exception("Operation not supported");
        }

        public bool InitLocals { get; set; }

        public void InsertSequencePoint(IFileSource file, int line, int position)
        {
        }
    }
}