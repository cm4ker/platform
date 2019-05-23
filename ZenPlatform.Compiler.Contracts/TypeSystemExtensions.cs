using System.Reflection.Emit;

namespace ZenPlatform.Compiler.Contracts
{
    public static class TypeSystemExtensions
    {
        public static IEmitter Ldarg(this IEmitter emitter, int arg)
            => emitter.Emit(OpCodes.Ldarg, arg);

        public static IEmitter Ldarg_0(this IEmitter emitter)
            => emitter.Emit(OpCodes.Ldarg_0);

        public static IEmitter Ldfld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Ldfld, field);

        public static IEmitter Ldsfld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Ldsfld, field);

        public static IEmitter LdThisFld(this IEmitter emitter, IField field)
            => emitter.Ldarg_0().Emit(OpCodes.Ldfld, field);

        public static IEmitter Stfld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Stfld, field);

        public static IEmitter Stsfld(this IEmitter emitter, IField field)
            => emitter.Emit(OpCodes.Stsfld, field);

        public static IEmitter Ldloc(this IEmitter emitter, ILocal local)
            => emitter.Emit(OpCodes.Ldloc, local);

        public static IEmitter Ldloca(this IEmitter emitter, ILocal local)
            => emitter.Emit(OpCodes.Ldloca, local);

        public static IEmitter Stloc(this IEmitter emitter, ILocal local)
            => emitter.Emit(OpCodes.Stloc, local);

        public static IEmitter Ldnull(this IEmitter emitter) => emitter.Emit(OpCodes.Ldnull);

        public static IEmitter Ldstr(this IEmitter emitter, string arg)
            => arg == null ? emitter.Ldnull() : emitter.Emit(OpCodes.Ldstr, arg);

        public static IEmitter Throw(this IEmitter emitter)
            => emitter.Emit(OpCodes.Throw);

        public static IEmitter Ldc_I4(this IEmitter emitter, int arg)
            => arg == 0
                ? emitter.Emit(OpCodes.Ldc_I4_0)
                : arg == 1
                    ? emitter.Emit(OpCodes.Ldc_I4_1)
                    : emitter.Emit(OpCodes.Ldc_I4, arg);

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

        public static IEmitter Brfalse(this IEmitter emitter, ILabel label)
            => emitter.Emit(OpCodes.Brfalse, label);

        public static IEmitter Brtrue(this IEmitter emitter, ILabel label)
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
        public static IEmitter Stelem_ref(this IEmitter emitter) => emitter.Emit(OpCodes.Stelem_Ref);
        public static IEmitter Ldlen(this IEmitter emitter) => emitter.Emit(OpCodes.Ldlen);

        public static IEmitter Add(this IEmitter emitter) => emitter.Emit(OpCodes.Add);
    }
}