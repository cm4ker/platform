using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public static class TypeExtensions
    {
        public static string GetFqn(this SreType type) => $"{type.Assembly?.Name}:{type.Namespace}.{type.Name}";

        public static string GetFullName(this SreType type)
        {
            var name = type.Name;
            if (type.Namespace != null)
                name = type.Namespace + "." + name;
            if (type.Assembly != null)
                name += "," + type.Assembly.Name;
            return name;
        }

        public static SreType GetType(this SreTypeSystem sys, string fullName)
        {
            var f = sys.FindType(fullName);
            if (f == null)
                throw new Exception($"Type not found: {fullName}");
            //throw new XamlIlTypeSystemException("Unable to resolve type " + type);
            return f;
        }

        public static IEnumerable<SreInvokableBase> FindMethods(this SreType type,
            Func<SreInvokableBase, bool> criteria)
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

        public static SreMethod FindMethod(this SreType type, Func<SreMethod, bool> criteria)
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

        public static SreMethod FindMethod(this SreType type, string name, params SreType[] args)
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

        public static SreInvokableBase FindMethod(this SreType type, string name, SreType returnType,
            bool allowDowncast, params SreType[] args)
        {
            foreach (var m in type.Methods)
            {
                // if (m.Name == name && m.ReturnType.Equals(returnType) && m.Parameters.Count == args.Length)
                // {
                //     var mismatch = false;
                //     for (var c = 0; c < args.Length; c++)
                //     {
                //         if (allowDowncast)
                //             mismatch = !m.Parameters[c].Type.IsAssignableFrom(args[c]);
                //         else
                //             mismatch = !m.Parameters[c].Equals(args[c]);
                //         if (mismatch)
                //             break;
                //     }
                //
                //     if (!mismatch)
                //         return m;
                // }
            }

            if (type.BaseType != null)
                return FindMethod(type.BaseType, name, returnType, allowDowncast, args);
            return null;
        }

        public static SreConstructor FindConstructor(this SreType type, List<SreType> args = null)
        {
            if (args == null)
                args = new List<SreType>();
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

        public static SreConstructor FindConstructor(this SreType type, params SreType[] args)
        {
            return FindConstructor(type, args.ToList());
        }

        public static bool IsNullable(this SreType type)
        {
            var def = type.GenericTypeDefinition;
            if (def == null) return false;
            return def.Namespace == "System" && def.Name == "Nullable`1";
        }

        public static bool IsNullableOf(this SreType type, SreType vtype)
        {
            return type.IsNullable() && type.GenericArguments[0].Equals(vtype);
        }

        public static SreType MakeGenericType(this SreType type, params SreType[] typeArguments)
            => type.MakeGenericType(typeArguments);

        public static bool IsAssignableFrom(this SreType to, SreType type)
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

        public static IEnumerable<SreType> GetAllInterfaces(this SreType type)
        {
            foreach (var i in type.Interfaces)
                yield return i;
            if (type.BaseType != null)
                foreach (var i in type.BaseType.GetAllInterfaces())
                    yield return i;
        }

        //     public static IEnumerable<SreProperty> GetAllProperties(this SreType t)
        //     {
        //         foreach (var p in t.Properties)
        //             yield return p;
        //         if (t.BaseType != null)
        //             foreach (var p in t.BaseType.GetAllProperties())
        //                 yield return p;
        //     }
        //
        public static SreProperty FindProperty(this SreType t, string name)
        {
            return FindProperty(t, (x => x.Name == name));
        }

        public static SreProperty FindProperty(this SreType t, Func<SreProperty, bool> criteria)
        {
            var result = t.Properties.FirstOrDefault(criteria);

            if (result == null && t.BaseType != null)
                return t.BaseType.FindProperty(criteria);

            return result;
        }


        public static SreField FindField(this SreType t, string name)
        {
            return FindField(t, (x => x.Name == name));
        }

        public static SreField FindField(this SreType t, Func<SreField, bool> criteria)
        {
            return t.Fields.FirstOrDefault(criteria);
        }

        //
        //     public static IEnumerable<SreField> GetAllFields(this SreType t)
        //     {
        //         foreach (var p in t.Fields)
        //             yield return p;
        //         if (t.BaseType != null)
        //             foreach (var p in t.BaseType.GetAllFields())
        //                 yield return p;
        //     }
        //
        //     public static IEnumerable<IEventInfo> GetAllEvents(this SreType t)
        //     {
        //         foreach (var p in t.Events)
        //             yield return p;
        //         if (t.BaseType != null)
        //             foreach (var p in t.BaseType.GetAllEvents())
        //                 yield return p;
        //     }
        //
        //     public static bool IsDirectlyAssignableFrom(this SreType type, SreType other)
        //     {
        //         if (type.IsValueType || other.IsValueType)
        //             return type.Equals(other);
        //         return type.IsAssignableFrom(other);
        //     }
        //
        //     public static SreType ThisOrFirstParameter(this SreMethodBase method) =>
        //         method.IsStatic ? method.Parameters[0].Type : method.DeclaringType;
        //
        //     public static IEmitter DebugHatch(this IEmitter emitter, string message)
        //     {
        //         return emitter;
        //     }
        //
        public static SrePropertyBuilder DefineProperty(this SreTypeBuilder tb, SreType type, string name,
            SreField backingField, bool interfaceImpl)
        {
            return DefineProperty(tb, type, name, backingField, true, true, interfaceImpl);
        }

        public static SrePropertyBuilder DefineProperty(this SreTypeBuilder tb, SreType type, string name,
            SreField backingField, bool hasGet, bool hasSet, bool interfaceImpl)
        {
            var result = tb.DefineProperty(type, name, false);
            if (hasGet)
            {
                var getMethod = tb.DefineMethod($"__get_{name}", true, false, interfaceImpl).WithReturnType(type);

                getMethod.Body
                    .LdArg_0()
                    .LdFld(backingField)
                    .Ret()
                    .Statement();

                result = result.WithGetter(getMethod);
            }

            if (hasSet)
            {
                var setMethod = tb.DefineMethod($"__set_{name}", true, false, interfaceImpl);
                var par = setMethod.DefineParameter("value", type, false, false);
                setMethod.Body.LdArg_0().LdArg(par).StFld(backingField).Statement();

                result = result.WithSetter(setMethod);
            }

            return result;
        }

        public static (SrePropertyBuilder prop, SreField field, SreMethodBuilder getMethod, SreMethodBuilder setMethod)
            DefineProperty(this SreTypeBuilder tb, SreType type, string name, bool hasGet, bool hasSet,
                bool interfaceImpl,
                SreProperty overrideProperty = null)
        {
            var backingField = tb.DefineField(type, ConventionsHelper.GetBackingFieldName(name), false, false);

            SreMethodBuilder getMethod = null, setMethod = null;

            var result = tb.DefineProperty(type, name, false);
            if (hasGet)
            {
                getMethod = tb.DefineMethod($"__get_{name}", true, false, interfaceImpl, overrideProperty?.Getter)
                    .WithReturnType(type);
                result = result.WithGetter(getMethod);
            }

            if (hasSet)
            {
                setMethod = tb.DefineMethod($"__set_{name}", true, false, interfaceImpl, overrideProperty?.Setter);
                setMethod.DefineParameter("value", type, false, false);

                result = result.WithSetter(setMethod);
            }

            return (result, backingField, getMethod, setMethod);
        }

        public static SrePropertyBuilder DefinePropertyWithBackingField(this SreTypeBuilder tb, SreType type,
            string name,
            bool interfaceImpl)
        {
            var backingField = tb.DefineField(type, ConventionsHelper.GetBackingFieldName(name), false, false);
            return tb.DefineProperty(type, name, backingField, interfaceImpl);
        }

        //     public static SreTypeBuilder DefineType(this SreAssemblyBuilder ab, string @namespace, string name,
        //         TypeAttributes attrs)
        //     {
        //         return ab.DefineType(@namespace, name, attrs, ab.TypeSystem.GetSystemBindings().Object);
        //     }
        //
        //
        public static SreConstructorBuilder DefineDefaultConstructor(this SreTypeBuilder tb, bool isStatic)
        {
            var c = tb.DefineConstructor(isStatic);
            // if (!isStatic)
            //     c.Body.LdArg_0().EmitCall(tb.BaseType.Constructors[0]).Ret();

            return c;
        }
    }

    public static class ConventionsHelper
    {
        public static string GetBackingFieldName(string name) => $"{name}k__BackingField";
    }

    public static class PropertyExtension
    {
        public static SreCustomAttribute FindCustomAttribute<T>(this SreProperty property)
        {
            var type = property.PropertyType.Assembly.TypeSystem.Resolve<T>();
            return property.FindCustomAttribute(type);
        }
    }
}