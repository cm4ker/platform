using System.Reflection.Emit;

namespace ZenPlatform.Compiler.Contracts
{
    public static class TypeSystemExtensions
    {
        public static IEmitter LdArg(this IEmitter emitter, int arg)
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
            else
                emitter.Emit(OpCodes.Ldarg_S, arg);

            return emitter;
        }

        public static IEmitter LdArg_0(this IEmitter emitter)
            => emitter.Emit(OpCodes.Ldarg_0);

        public static IEmitter LdFld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Ldfld, field);

        public static IEmitter LdFldA(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Ldflda, field);

        public static IEmitter LdsFld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Ldsfld, field);

        public static IEmitter LdsFldA(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Ldsflda, field);

        public static IEmitter LdArgA(this IEmitter emitter, IParameter parameter)
            => emitter.Emit(OpCodes.Ldarga, parameter.Sequence - 1);

        public static IEmitter LdThisFld(this IEmitter emitter, IField field)
            => emitter.LdArg_0().Emit(OpCodes.Ldfld, field);

        public static IEmitter Stfld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Stfld, field);

        public static IEmitter Stsfld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Stsfld, field);

        public static IEmitter LdLoc(this IEmitter emitter, ILocal local)
            => emitter.Emit(OpCodes.Ldloc, local);

        public static IEmitter LdLocA(this IEmitter emitter, ILocal local)
            => emitter.Emit(OpCodes.Ldloca, local);

        public static IEmitter StLoc(this IEmitter emitter, ILocal local)
            => emitter.Emit(OpCodes.Stloc, local);

        public static IEmitter Ldnull(this IEmitter emitter) => emitter.Emit(OpCodes.Ldnull);

        public static IEmitter Ldstr(this IEmitter emitter, string arg)
            => arg == null ? emitter.Ldnull() : emitter.Emit(OpCodes.Ldstr, arg);

        public static IEmitter Throw(this IEmitter emitter)
            => emitter.Emit(OpCodes.Throw);

        public static IEmitter LdcI4(this IEmitter emitter, int arg)
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

        public static IEmitter Beq(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Beq, label);

        public static IEmitter Blt(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Blt, label);

        public static IEmitter Ble(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Ble, label);

        public static IEmitter Bgt(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Bgt, label);

        public static IEmitter Bge(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Bge, label);

        public static IEmitter Br(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Br, label);

        public static IEmitter BrFalse(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Brfalse, label);

        public static IEmitter BrTrue(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Brtrue, label);

        public static IEmitter Ret(this IEmitter emitter)
            => emitter.Emit(OpCodes.Ret);

        public static IEmitter Dup(this IEmitter emitter)
            => emitter.Emit(OpCodes.Dup);

        public static IEmitter Pop(this IEmitter emitter)
            => emitter.Emit(OpCodes.Pop);

        public static IEmitter Ldtoken(this IEmitter emitter, IType type)
            => emitter.Emit(OpCodes.Ldtoken, type);

        public static IEmitter Ldtoken(this IEmitter emitter, IMethod method)
            => emitter.Emit(OpCodes.Ldtoken, method);

        public static IEmitter Ldtype(this IEmitter emitter, IType type)
        {
            var conv = emitter.TypeSystem.GetType("System.Type")
                .FindMethod(m => m.IsStatic && m.IsPublic && m.Name == "GetTypeFromHandle");
            return emitter.Ldtoken(type).EmitCall(conv);
        }

        public static IEmitter LdMethodInfo(this IEmitter emitter, IMethod method)
        {
            var conv = emitter.TypeSystem.GetType("System.Reflection.MethodInfo")
                .FindMethod(m => m.IsStatic && m.IsPublic && m.Name == "GetMethodFromHandle");
            return emitter.Ldtoken(method).EmitCall(conv);
        }

        public static IEmitter Ldftn(this IEmitter emitter, IMethod method)
            => emitter.Emit(OpCodes.Ldftn, method);

        public static IEmitter Isinst(this IEmitter emitter, IType type)
            => emitter.Emit(OpCodes.Isinst, type);

        public static IEmitter Castclass(this IEmitter emitter, IType type)
            => emitter.Emit(OpCodes.Castclass, type);

        public static IEmitter Box(this IEmitter emitter, IType type)
            => emitter.Emit(OpCodes.Box, type);

        public static IEmitter Unbox_Any(this IEmitter emitter, IType type)
            => emitter.Emit(OpCodes.Unbox_Any, type);

        public static IEmitter Newobj(this IEmitter emitter, IConstructor ctor)
            => emitter.Emit(OpCodes.Newobj, ctor);

        public static IEmitter Newarr(this IEmitter emitter, IType type)
            => emitter.Emit(OpCodes.Newarr, type);

        public static IEmitter Ldelem_ref(this IEmitter emitter) => emitter.Emit(OpCodes.Ldelem_Ref);

        public static IEmitter LdElemA(this IEmitter emitter) => emitter.Emit(OpCodes.Ldelema);
        public static IEmitter Stelem_ref(this IEmitter emitter) => emitter.Emit(OpCodes.Stelem_Ref);
        public static IEmitter Ldlen(this IEmitter emitter) => emitter.Emit(OpCodes.Ldlen);

        public static IEmitter Add(this IEmitter emitter) => emitter.Emit(OpCodes.Add);
        public static IEmitter Sub(this IEmitter emitter) => emitter.Emit(OpCodes.Sub);
        public static IEmitter Mul(this IEmitter emitter) => emitter.Emit(OpCodes.Mul);
        public static IEmitter Div(this IEmitter emitter) => emitter.Emit(OpCodes.Div);
        public static IEmitter Ceq(this IEmitter emitter) => emitter.Emit(OpCodes.Ceq);
        public static IEmitter Or(this IEmitter emitter) => emitter.Emit(OpCodes.Or);

        public static IEmitter Neg(this IEmitter emitter) => emitter.Emit(OpCodes.Neg);
        public static IEmitter Not(this IEmitter emitter) => emitter.Emit(OpCodes.Not);

        public static IEmitter NotEqual(this IEmitter e) => e.Ceq().LdcI4(0).Ceq();

        public static IEmitter Rem(this IEmitter emitter) => emitter.Emit(OpCodes.Rem);
        public static IEmitter Clt(this IEmitter emitter) => emitter.Emit(OpCodes.Clt);
        public static IEmitter Cgt(this IEmitter emitter) => emitter.Emit(OpCodes.Cgt);

        public static IEmitter GreaterOrEqual(this IEmitter e) => e.Clt().LdcI4(0).Ceq();
        public static IEmitter LessOrEqual(this IEmitter e) => e.Cgt().LdcI4(0).Ceq();

        public static IEmitter NewArr(this IEmitter emitter, IType type) => emitter.Emit(OpCodes.Newarr, type);

        public static IEmitter StElemI4(this IEmitter emitter) => emitter.Emit(OpCodes.Stelem_I4);
        public static IEmitter LdElemI4(this IEmitter emitter) => emitter.Emit(OpCodes.Ldelem_I4);

        public static IEmitter LdIndI4(this IEmitter emitter) => emitter.Emit(OpCodes.Ldind_I4);
        public static IEmitter StIndI4(this IEmitter emitter) => emitter.Emit(OpCodes.Stind_I4);

        public static IEmitter ConvI4(this IEmitter emitter) => emitter.Emit(OpCodes.Conv_I4);
        public static IEmitter ConvR4(this IEmitter emitter) => emitter.Emit(OpCodes.Conv_R4);
        public static IEmitter ConvR8(this IEmitter emitter) => emitter.Emit(OpCodes.Conv_R8);
        public static IEmitter ConvU2(this IEmitter emitter) => emitter.Emit(OpCodes.Conv_U2);

        public static IEmitter LdStr(this IEmitter emitter, string str) => emitter.Emit(OpCodes.Ldstr, str);

        public static IEmitter LdcR8(this IEmitter emitter, double value) => emitter.Emit(OpCodes.Ldc_R8, value);

        public static IEmitter StArg(this IEmitter emitter, IParameter parameter) =>
            emitter.Emit(OpCodes.Stelem_I4, parameter);
    }
}