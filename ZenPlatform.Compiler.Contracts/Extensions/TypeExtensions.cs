using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ZenPlatform.Compiler.Contracts.Extensions
{
    public static class TypeExtensions
    {
        public static string GetFqn(this IType type) => $"{type.Assembly?.Name}:{type.Namespace}.{type.Name}";

        public static string GetFullName(this IType type)
        {
            var name = type.Name;
            if (type.Namespace != null)
                name = type.Namespace + "." + name;
            if (type.Assembly != null)
                name += "," + type.Assembly.Name;
            return name;
        }

        public static IType GetType(this ITypeSystem sys, string fullName)
        {
            var f = sys.FindType(fullName);
            if (f == null)
                throw new Exception($"Type not found: {fullName}");
            //throw new XamlIlTypeSystemException("Unable to resolve type " + type);
            return f;
        }

        public static IEnumerable<IMethod> FindMethods(this IType type, Func<IMethod, bool> criteria)
        {
            foreach (var m in type.Methods)
                if (criteria(m))
                    yield return m;
            var bt = type.BaseType;
            if (bt != null)
                foreach (var bm in bt.FindMethods(criteria))
                    yield return bm;
            foreach (var iface in type.Interfaces)
            foreach (var m in iface.Methods)
                if (criteria(m))
                    yield return m;
        }

        public static IMethod FindMethod(this IType type, Func<IMethod, bool> criteria)
        {
            foreach (var m in type.Methods)
                if (criteria(m))
                    return m;
            var bres = type.BaseType?.FindMethod(criteria);
            if (bres != null)
                return bres;
            foreach (var iface in type.Interfaces)
            foreach (var m in iface.Methods)
                if (criteria(m))
                    return m;
            return null;
        }

        public static IMethod FindMethod(this IType type, string name, params IType[] args)
        {
            return type.FindMethod(m =>
            {
                if (m.Name == name && m.Parameters.Count == args.Length)
                {
                    var mismatch = false;
                    for (var c = 0; c < args.Length; c++)
                    {
                        mismatch = !m.Parameters[c].Type.Equals(args[c]);
                        if (mismatch)
                            break;
                    }

                    if (!mismatch)
                        return true;
                }

                return false;
            });
        }

        public static IMethod FindMethod(this IType type, string name, IType returnType,
            bool allowDowncast, params IType[] args)
        {
            foreach (var m in type.Methods)
            {
                if (m.Name == name && m.ReturnType.Equals(returnType) && m.Parameters.Count == args.Length)
                {
                    var mismatch = false;
                    for (var c = 0; c < args.Length; c++)
                    {
                        if (allowDowncast)
                            mismatch = !m.Parameters[c].Type.IsAssignableFrom(args[c]);
                        else
                            mismatch = !m.Parameters[c].Equals(args[c]);
                        if (mismatch)
                            break;
                    }

                    if (!mismatch)
                        return m;
                }
            }

            if (type.BaseType != null)
                return FindMethod(type.BaseType, name, returnType, allowDowncast, args);
            return null;
        }

        public static IConstructor FindConstructor(this IType type, List<IType> args = null)
        {
            if (args == null)
                args = new List<IType>();
            foreach (var ctor in type.Constructors.Where(c => c.IsPublic
                                                              && !c.IsStatic
                                                              && c.Parameters.Count == args.Count))
            {
                var mismatch = false;

                for (var c = 0; c < args.Count; c++)
                {
                    mismatch = !ctor.Parameters[c].Type.IsAssignableFrom(args[c]);
                    if (mismatch)
                        break;
                }

                if (!mismatch)
                    return ctor;
            }

            return null;
        }

        public static IConstructor FindConstructor(this IType type, params IType[] args)
        {
            return FindConstructor(type, args.ToList());
        }

        public static bool IsNullable(this IType type)
        {
            var def = type.GenericTypeDefinition;
            if (def == null) return false;
            return def.Namespace == "System" && def.Name == "Nullable`1";
        }

        public static bool IsNullableOf(this IType type, IType vtype)
        {
            return type.IsNullable() && type.GenericArguments[0].Equals(vtype);
        }

        public static IEmitter EmitCall(this IEmitter emitter, IMethod method,
            bool swallowResult = false)
        {
            if (method is ICustomEmitMethod custom)
                custom.EmitCall(emitter);
            else
            {
                emitter.Emit(method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, method);
            }

            if (swallowResult && !(method.ReturnType.Namespace == "System" && method.ReturnType.Name == "Void"))
                emitter.Pop();

            return emitter;
        }

        public static IEmitter EmitCall(this IEmitter emitter, IConstructor method,
            bool swallowResult = false)
        {
            emitter.Emit(OpCodes.Call, method);
            return emitter;
        }

        public static IType MakeGenericType(this IType type, params IType[] typeArguments)
            => type.MakeGenericType(typeArguments);

        public static bool IsAssignableFrom(this IType to, IType type)
        {
            if (type.IsValueType
                && to.GenericTypeDefinition?.FullName == "System.Nullable`1"
                && to.GenericArguments[0].Equals(type))
                return true;
            if (to.FullName == "System.Object" && type.IsInterface)
                return true;
            var baseType = type;
            while (baseType != null)
            {
                if (baseType.Equals(to))
                    return true;
                baseType = baseType.BaseType;
            }

            if (to.IsInterface && type.GetAllInterfaces().Any(to.IsAssignableFrom))
                return true;
            return false;
        }

        public static IEnumerable<IType> GetAllInterfaces(this IType type)
        {
            foreach (var i in type.Interfaces)
                yield return i;
            if (type.BaseType != null)
                foreach (var i in type.BaseType.GetAllInterfaces())
                    yield return i;
        }

        public static IEnumerable<IProperty> GetAllProperties(this IType t)
        {
            foreach (var p in t.Properties)
                yield return p;
            if (t.BaseType != null)
                foreach (var p in t.BaseType.GetAllProperties())
                    yield return p;
        }

        public static IProperty FindProperty(this IType t, string name)
        {
            return FindProperty(t, (x => x.Name == name));
        }

        public static IProperty FindProperty(this IType t, Func<IProperty, bool> criteria)
        {
            var result = t.Properties.FirstOrDefault(criteria);

            if (result == null && t.BaseType != null)
                return t.BaseType.FindProperty(criteria);

            return result;
        }

        public static IField FindField(this IType t, string name)
        {
            return FindField(t, (x => x.Name == name));
        }

        public static IField FindField(this IType t, Func<IField, bool> criteria)
        {
            return t.Fields.FirstOrDefault(criteria);
        }

        public static IEnumerable<IField> GetAllFields(this IType t)
        {
            foreach (var p in t.Fields)
                yield return p;
            if (t.BaseType != null)
                foreach (var p in t.BaseType.GetAllFields())
                    yield return p;
        }

        public static IEnumerable<IEventInfo> GetAllEvents(this IType t)
        {
            foreach (var p in t.Events)
                yield return p;
            if (t.BaseType != null)
                foreach (var p in t.BaseType.GetAllEvents())
                    yield return p;
        }

        public static bool IsDirectlyAssignableFrom(this IType type, IType other)
        {
            if (type.IsValueType || other.IsValueType)
                return type.Equals(other);
            return type.IsAssignableFrom(other);
        }

        public static IType ThisOrFirstParameter(this IMethod method) =>
            method.IsStatic ? method.Parameters[0].Type : method.DeclaringType;

        public static IEmitter DebugHatch(this IEmitter emitter, string message)
        {
            return emitter;
        }

        public static IPropertyBuilder DefineProperty(this ITypeBuilder tb, IType type, string name,
            IField backingField, bool interfaceImpl)
        {
            return DefineProperty(tb, type, name, backingField, true, true, interfaceImpl);
        }

        public static IPropertyBuilder DefineProperty(this ITypeBuilder tb, IType type, string name,
            IField backingField, bool hasGet, bool hasSet, bool interfaceImpl)
        {
            var result = tb.DefineProperty(type, name, false);
            if (hasGet)
            {
                var getMethod = tb.DefineMethod($"get_{name}", true, false, interfaceImpl).WithReturnType(type);

                getMethod.Generator
                    .LdArg_0()
                    .LdFld(backingField)
                    .Ret();

                result = result.WithGetter(getMethod);
            }

            if (hasSet)
            {
                var setMethod = tb.DefineMethod($"set_{name}", true, false, interfaceImpl);
                setMethod.DefineParameter("value", type, false, false);
                setMethod.Generator.LdArg(0).LdArg(1).StFld(backingField).Ret();

                result = result.WithSetter(setMethod);
            }

            return result;
        }

        public static (IPropertyBuilder prop, IField field, IMethodBuilder getMethod, IMethodBuilder setMethod)
            DefineProperty(this ITypeBuilder tb, IType type, string name, bool hasGet, bool hasSet, bool interfaceImpl,
                IProperty overrideProperty = null)
        {
            var backingField = tb.DefineField(type, ConventionsHelper.GetBackingFieldName(name), false, false);

            IMethodBuilder getMethod = null, setMethod = null;

            var result = tb.DefineProperty(type, name, false);
            if (hasGet)
            {
                getMethod = tb.DefineMethod($"get_{name}", true, false, interfaceImpl, overrideProperty?.Getter)
                    .WithReturnType(type);
                result = result.WithGetter(getMethod);
            }

            if (hasSet)
            {
                setMethod = tb.DefineMethod($"set_{name}", true, false, interfaceImpl, overrideProperty?.Setter);
                setMethod.DefineParameter("value", type, false, false);

                result = result.WithSetter(setMethod);
            }

            return (result, backingField, getMethod, setMethod);
        }

        public static IPropertyBuilder DefinePropertyWithBackingField(this ITypeBuilder tb, IType type, string name,
            bool interfaceImpl)
        {
            var backingField = tb.DefineField(type, ConventionsHelper.GetBackingFieldName(name), false, false);
            return tb.DefineProperty(type, name, backingField, interfaceImpl);
        }

        public static ITypeBuilder DefineType(this IAssemblyBuilder ab, string @namespace, string name,
            TypeAttributes attrs)
        {
            return ab.DefineType(@namespace, name, attrs, ab.TypeSystem.GetSystemBindings().Object);
        }


        public static IConstructorBuilder DefineDefaultConstructor(this ITypeBuilder tb, bool isStatic)
        {
            var c = tb.DefineConstructor(isStatic);
            if (!isStatic)
                c.Generator.LdArg_0().EmitCall(tb.BaseType.Constructors[0]).Ret();

            return c;
        }
    }

    public static class PropertyExtension
    {
        public static ICustomAttribute FindCustomAttribute<T>(this IProperty property)
        {
            var type = property.PropertyType.Assembly.TypeSystem.FindType<T>();
            return property.FindCustomAttribute(type);
        }
    }

    public static class ConventionsHelper
    {
        public static string GetBackingFieldName(string name) => $"<{name}>k__BackingField";
    }
}