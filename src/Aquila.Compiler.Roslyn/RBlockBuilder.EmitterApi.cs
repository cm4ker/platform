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

            if (code == OpCodes.Ceq)
                return Ceq();

            if (code == OpCodes.Ldc_I4_0)
                return LdLit(0);
            if (code == OpCodes.Ldc_I4_1)
                return LdLit(1);
            if (code == OpCodes.Ldc_I4_2)
                return LdLit(2);
            if (code == OpCodes.Ldc_I4_3)
                return LdLit(3);
            if (code == OpCodes.Ldc_I4_4)
                return LdLit(4);
            if (code == OpCodes.Ldc_I4_5)
                return LdLit(5);
            if (code == OpCodes.Ldc_I4_6)
                return LdLit(6);
            if (code == OpCodes.Ldc_I4_7)
                return LdLit(7);
            if (code == OpCodes.Ldc_I4_8)
                return LdLit(8);

            if (code == OpCodes.Add)
                return Add();

            if (code == OpCodes.Clt)
                return Clt();


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
                return Goto(label);
            }

            if (code == OpCodes.Br_S)
            {
                return Goto(label);
            }

            if (code == OpCodes.Brfalse)
            {
                return Not().Block()
                    .Goto(label)
                    .EndBlock()
                    .Nothing()
                    .If();
            }

            if (code == OpCodes.Brtrue)
            {
                return Block()
                    .Goto(label)
                    .EndBlock()
                    .Nothing()
                    .If();
            }

            throw new Exception("Operation not supported");
        }

        public IEmitter Emit(OpCode code, ILocal local)
        {
            if (code == OpCodes.Ldloc)
                return LdLoc(local);

            if (code == OpCodes.Stloc)
                return StLoc(local);

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