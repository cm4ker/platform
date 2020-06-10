using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PCilBody
    {
        public List<PLabel> Labels { get; } = new List<PLabel>();

        public List<PLocal> Locals { get; } = new List<PLocal>();

        public PCilBody()
        {
            Instructions = new List<PInstruction>();
        }

        public List<PInstruction> Instructions { get; }

        public PCilBody Emit(OpCode code)
        {
            Instructions.Add(PInstruction.Create(code));
            return this;
        }

        public PCilBody Emit(OpCode code, PField field)
        {
            Instructions.Add(PInstruction.Create(code, field));
            return this;
        }

        public PCilBody Emit(OpCode code, PMethod method)
        {
            Instructions.Add(PInstruction.Create(code, method));
            return this;
        }

        public PCilBody Emit(OpCode code, PConstructor ctor)
        {
            Instructions.Add(PInstruction.Create(code, ctor));
            return this;
        }

        public PCilBody Emit(OpCode code, string arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public PCilBody Emit(OpCode code, int arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public PCilBody Emit(OpCode code, long arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public PCilBody Emit(OpCode code, PType type)
        {
            Instructions.Add(PInstruction.Create(code, type));
            return this;
        }

        public PCilBody Emit(OpCode code, float arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public PCilBody Emit(OpCode code, double arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public PLocal DefineLocal(PType type)
        {
            var local = new PLocal(type, 0);

            Locals.Add(local);

            return local;
        }

        public PLabel DefineLabel()
        {
            var l = new PLabel();
            Labels.Add(l);
            return l;
        }

        public PCilBody MarkLabel(PLabel label)
        {
            Instructions.Add(label.Instruction);
            return this;
        }

        public PCilBody Emit(OpCode code, PLabel label)
        {
            Instructions.Add(PInstruction.Create(code, label.Instruction));
            return this;
        }

        public PCilBody Emit(OpCode code, PLocal local)
        {
            Instructions.Add(PInstruction.Create(code, local));
            return this;
        }

        public PCilBody Emit(OpCode code, PParameter parameter)
        {
            Instructions.Add(PInstruction.Create(code, parameter));
            return this;
        }
    }

    public static class EmitterExtensions
    {
        public static PCilBody LdArg(this PCilBody emitter, int arg)
        {
            if (arg < 4)
                switch (arg)
                {
                    case 0:
                        emitter.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        emitter.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        emitter.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        emitter.Emit(OpCodes.Ldarg_3);
                        break;
                }
            else if (arg < 255)
                emitter.Emit(OpCodes.Ldarg_S, arg);
            else
                emitter.Emit(OpCodes.Ldarg, arg);

            return emitter;
        }

        public static PCilBody LdArg(this PCilBody emitter, PParameter parameter)
        {
            return LdArg(emitter, parameter);
        }

        public static PCilBody LdArg_0(this PCilBody emitter)
            => emitter.Emit(OpCodes.Ldarg_0);

        public static PCilBody LdFld(this PCilBody emitter, PField field)
            => emitter.Emit(OpCodes.Ldfld, field);

        public static PCilBody LdFldA(this PCilBody emitter, PField field)
            => emitter.Emit(OpCodes.Ldflda, field);

        public static PCilBody LdSFld(this PCilBody emitter, PField field)
            => emitter.Emit(OpCodes.Ldsfld, field);

        public static PCilBody LdsFldA(this PCilBody emitter, PField field)
            => emitter.Emit(OpCodes.Ldsflda, field);

        public static PCilBody LdArgA(this PCilBody emitter, PParameter parameter)
            => emitter.Emit(OpCodes.Ldarga, parameter);

        public static PCilBody LdThisFld(this PCilBody emitter, PField field)
            => emitter.LdArg_0().Emit(OpCodes.Ldfld, field);

        public static PCilBody StFld(this PCilBody emitter, PField field)
            => emitter.Emit(OpCodes.Stfld, field);

        public static PCilBody StSFld(this PCilBody emitter, PField field)
            => emitter.Emit(OpCodes.Stsfld, field);

        public static PCilBody LdLoc(this PCilBody emitter, PLocal local)
            => emitter.Emit(OpCodes.Ldloc, local);

        public static PCilBody LdLocA(this PCilBody emitter, PLocal local)
            => emitter.Emit(OpCodes.Ldloca, local);

        public static PCilBody StLoc(this PCilBody emitter, PLocal local)
            => emitter.Emit(OpCodes.Stloc, local);

        public static PCilBody Leave(this PCilBody emitter, PLocal label) =>
            emitter.Emit(OpCodes.Leave_S, label);

        public static PCilBody LdNull(this PCilBody emitter) => emitter.Emit(OpCodes.Ldnull);

        public static PCilBody LdStr(this PCilBody emitter, string arg)
            => arg == null ? emitter.LdNull() : emitter.Emit(OpCodes.Ldstr, arg);

        public static PCilBody Throw(this PCilBody emitter)
            => emitter.Emit(OpCodes.Throw);

        public static PCilBody Throw(this PCilBody emitter, PType type)
        {
            var con = type.Constructors.FirstOrDefault(x => !x.Parameters.Any());

            if (con == null)
                throw new Exception("Exception haven't default constructor use NewObj + Throw instead");

            return emitter.NewObj(con).Emit(OpCodes.Throw);
        }


        public static PCilBody LdcI4(this PCilBody emitter, int arg)
        {
            switch (arg)
            {
                case 0: return emitter.Emit(OpCodes.Ldc_I4_0);
                case 1: return emitter.Emit(OpCodes.Ldc_I4_1);
                case 2: return emitter.Emit(OpCodes.Ldc_I4_2);
                case 3: return emitter.Emit(OpCodes.Ldc_I4_3);
                case 4: return emitter.Emit(OpCodes.Ldc_I4_4);
                case 5: return emitter.Emit(OpCodes.Ldc_I4_5);
                case 6: return emitter.Emit(OpCodes.Ldc_I4_6);
                case 7: return emitter.Emit(OpCodes.Ldc_I4_7);
                case 8: return emitter.Emit(OpCodes.Ldc_I4_8);
                default: return emitter.Emit(OpCodes.Ldc_I4, arg);
            }
        }

        public static PCilBody Beq(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Beq, label);

        public static PCilBody Blt(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Blt, label);

        public static PCilBody Ble(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Ble, label);

        public static PCilBody Bgt(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Bgt, label);

        public static PCilBody Bge(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Bge, label);

        public static PCilBody Br(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Br, label);

        public static PCilBody BrFalse(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Brfalse, label);

        public static PCilBody BrTrue(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Brtrue, label);

        public static PCilBody BneUn(this PCilBody emitter, PLabel label)
            => emitter.Emit(OpCodes.Bne_Un, label);


        public static PCilBody Ret(this PCilBody emitter)
            => emitter.Emit(OpCodes.Ret);

        public static PCilBody Dup(this PCilBody emitter)
            => emitter.Emit(OpCodes.Dup);

        public static PCilBody Pop(this PCilBody emitter)
            => emitter.Emit(OpCodes.Pop);

        public static PCilBody LdToken(this PCilBody emitter, PType type)
            => emitter.Emit(OpCodes.Ldtoken, type);

        public static PCilBody LdToken(this PCilBody emitter, PMethod method)
            => emitter.Emit(OpCodes.Ldtoken, method);

        // public static PCilBody LdType(this PCilBody emitter, PType type)
        // {
        //     var conv = emitter.TypeSystem.GetType("System.Type")
        //         .FindMethod(m => m.IsStatic && m.IsPublic && m.Name == "GetTypeFromHandle");
        //     return emitter.LdToken(type).EmitCall(conv);
        // }

        // public static PCilBody LdMethodInfo(this PCilBody emitter, IMethod method)
        // {
        //     var conv = emitter.TypeSystem.GetType("System.Reflection.MethodInfo")
        //         .FindMethod(m => m.IsStatic && m.IsPublic && m.Name == "GetMethodFromHandle");
        //     return emitter.LdToken(method).EmitCall(conv);
        // }

        public static PCilBody LdFtn(this PCilBody emitter, PMethod method)
            => emitter.Emit(OpCodes.Ldftn, method);

        public static PCilBody IsInst(this PCilBody emitter, PType type)
            => emitter.Emit(OpCodes.Isinst, type);

        public static PCilBody Cast(this PCilBody emitter, PType type)
            => emitter.Emit(OpCodes.Castclass, type);

        public static PCilBody Box(this PCilBody emitter, PType type)
            => emitter.Emit(OpCodes.Box, type);

        public static PCilBody Unbox_Any(this PCilBody emitter, PType type)
            => emitter.Emit(OpCodes.Unbox_Any, type);

        public static PCilBody NewObj(this PCilBody emitter, PConstructor ctor)
            => emitter.Emit(OpCodes.Newobj, ctor);


        public static PCilBody LdElemRef(this PCilBody emitter) => emitter.Emit(OpCodes.Ldelem_Ref);

        public static PCilBody LdElemA(this PCilBody emitter) => emitter.Emit(OpCodes.Ldelema);
        public static PCilBody StElemRef(this PCilBody emitter) => emitter.Emit(OpCodes.Stelem_Ref);
        public static PCilBody LdLen(this PCilBody emitter) => emitter.Emit(OpCodes.Ldlen);

        public static PCilBody Add(this PCilBody emitter) => emitter.Emit(OpCodes.Add);
        public static PCilBody And(this PCilBody emitter) => emitter.Emit(OpCodes.And);
        public static PCilBody Sub(this PCilBody emitter) => emitter.Emit(OpCodes.Sub);
        public static PCilBody Mul(this PCilBody emitter) => emitter.Emit(OpCodes.Mul);
        public static PCilBody Div(this PCilBody emitter) => emitter.Emit(OpCodes.Div);
        public static PCilBody Ceq(this PCilBody emitter) => emitter.Emit(OpCodes.Ceq);
        public static PCilBody Or(this PCilBody emitter) => emitter.Emit(OpCodes.Or);

        public static PCilBody Neg(this PCilBody emitter) => emitter.Emit(OpCodes.Neg);
        public static PCilBody Not(this PCilBody emitter) => emitter.Emit(OpCodes.Not);

        public static PCilBody Cneq(this PCilBody e) => e.Ceq().LdcI4(0).Ceq();

        public static PCilBody Rem(this PCilBody emitter) => emitter.Emit(OpCodes.Rem);
        public static PCilBody Clt(this PCilBody emitter) => emitter.Emit(OpCodes.Clt);
        public static PCilBody Cgt(this PCilBody emitter) => emitter.Emit(OpCodes.Cgt);

        public static PCilBody GreaterOrEqual(this PCilBody e) => e.Clt().LdcI4(0).Ceq();
        public static PCilBody LessOrEqual(this PCilBody e) => e.Cgt().LdcI4(0).Ceq();

        public static PCilBody NewArr(this PCilBody emitter, PType type) => emitter.Emit(OpCodes.Newarr, type);

        public static PCilBody StElemI4(this PCilBody emitter) => emitter.Emit(OpCodes.Stelem_I4);
        public static PCilBody LdElemI4(this PCilBody emitter) => emitter.Emit(OpCodes.Ldelem_I4);

        public static PCilBody LdIndI4(this PCilBody emitter) => emitter.Emit(OpCodes.Ldind_I4);
        public static PCilBody LdIndRef(this PCilBody emitter) => emitter.Emit(OpCodes.Ldind_Ref);
        public static PCilBody StIndI4(this PCilBody emitter) => emitter.Emit(OpCodes.Stind_I4);
        public static PCilBody StIndRef(this PCilBody emitter) => emitter.Emit(OpCodes.Stind_Ref);

        public static PCilBody ConvI4(this PCilBody emitter) => emitter.Emit(OpCodes.Conv_I4);
        public static PCilBody ConvR4(this PCilBody emitter) => emitter.Emit(OpCodes.Conv_R4);
        public static PCilBody ConvR8(this PCilBody emitter) => emitter.Emit(OpCodes.Conv_R8);
        public static PCilBody ConvU2(this PCilBody emitter) => emitter.Emit(OpCodes.Conv_U2);

//        public static PCilBody LdStr(this PCilBody emitter, string str) => emitter.Emit(OpCodes.Ldstr, str);

        public static PCilBody LdcR8(this PCilBody emitter, double value) => emitter.Emit(OpCodes.Ldc_R8, value);

        public static PCilBody StArg(this PCilBody emitter, PParameter parameter) =>
            emitter.Emit(OpCodes.Starg_S, parameter);

        public static PCilBody Call(this PCilBody emitter, PMethod m) =>
            emitter.Emit(OpCodes.Call, m);

        public static PCilBody Call(this PCilBody emitter, PConstructor m) =>
            emitter.Emit(OpCodes.Call, m);

        public static PCilBody LdProp(this PCilBody emitter, PProperty prop) => emitter.Call(prop.Getter);

        public static PCilBody StProp(this PCilBody emitter, PProperty prop) => emitter.Call(prop.Setter);


        public static PCilBody LdLit(this PCilBody emitter, int i) => emitter.LdcI4(i);
        public static PCilBody LdLit(this PCilBody emitter, long i) => emitter.LdcR8(i);
        public static PCilBody LdLit(this PCilBody emitter, double i) => emitter.LdcR8(i);
        public static PCilBody LdLit(this PCilBody emitter, string i) => emitter.LdStr(i);

        public static PCilBody LdElem(this PCilBody emitter) => emitter.Emit(OpCodes.Ldelem);
    }
}