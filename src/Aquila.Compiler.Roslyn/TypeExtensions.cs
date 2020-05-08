using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public static class TypeExtensions
    {
        public static string GetFqn(this RoslynType type) => $"{type.Assembly?.Name}:{type.Namespace}.{type.Name}";

        public static string GetFullName(this RoslynType type)
        {
            var name = type.Name;
            if (type.Namespace != null)
                name = type.Namespace + "." + name;
            if (type.Assembly != null)
                name += "," + type.Assembly.Name;
            return name;
        }

        public static RoslynType GetType(this RoslynTypeSystem sys, string fullName)
        {
            var f = sys.FindType(fullName);
            if (f == null)
                throw new Exception($"Type not found: {fullName}");
            //throw new XamlIlTypeSystemException("Unable to resolve type " + type);
            return f;
        }

        public static IEnumerable<RoslynInvokableBase> FindMethods(this RoslynType type,
            Func<RoslynInvokableBase, bool> criteria)
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

        public static RoslynMethod FindMethod(this RoslynType type, Func<RoslynMethod, bool> criteria)
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

        public static RoslynMethod FindMethod(this RoslynType type, string name, params RoslynType[] args)
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

        public static RoslynInvokableBase FindMethod(this RoslynType type, string name, RoslynType returnType,
            bool allowDowncast, params RoslynType[] args)
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

        public static RoslynConstructor FindConstructor(this RoslynType type, List<RoslynType> args = null)
        {
            if (args == null)
                args = new List<RoslynType>();
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

        public static RoslynConstructor FindConstructor(this RoslynType type, params RoslynType[] args)
        {
            return FindConstructor(type, args.ToList());
        }

        public static bool IsNullable(this RoslynType type)
        {
            var def = type.GenericTypeDefinition;
            if (def == null) return false;
            return def.Namespace == "System" && def.Name == "Nullable`1";
        }

        public static bool IsNullableOf(this RoslynType type, RoslynType vtype)
        {
            return type.IsNullable() && type.GenericArguments[0].Equals(vtype);
        }

        public static RoslynType MakeGenericType(this RoslynType type, params RoslynType[] typeArguments)
            => type.MakeGenericType(typeArguments);

        public static bool IsAssignableFrom(this RoslynType to, RoslynType type)
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

        public static IEnumerable<RoslynType> GetAllInterfaces(this RoslynType type)
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
        public static RoslynProperty FindProperty(this RoslynType t, string name)
        {
            return FindProperty(t, (x => x.Name == name));
        }

        public static RoslynProperty FindProperty(this RoslynType t, Func<RoslynProperty, bool> criteria)
        {
            var result = t.Properties.FirstOrDefault(criteria);

            if (result == null && t.BaseType != null)
                return t.BaseType.FindProperty(criteria);

            return result;
        }


        public static RoslynField FindField(this RoslynType t, string name)
        {
            return FindField(t, (x => x.Name == name));
        }

        public static RoslynField FindField(this RoslynType t, Func<RoslynField, bool> criteria)
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
        public static RoslynPropertyBuilder DefineProperty(this RoslynTypeBuilder tb, RoslynType type, string name,
            RoslynField backingField, bool interfaceImpl)
        {
            return DefineProperty(tb, type, name, backingField, true, true, interfaceImpl);
        }

        public static RoslynPropertyBuilder DefineProperty(this RoslynTypeBuilder tb, RoslynType type, string name,
            RoslynField backingField, bool hasGet, bool hasSet, bool interfaceImpl)
        {
            var result = tb.DefineProperty(type, name, false);
            if (hasGet)
            {
                var getMethod = tb.DefineMethod($"__get_{name}", true, false, interfaceImpl).WithReturnType(type);

                getMethod.Body
                    .LdArg_0()
                    .LdFld(backingField)
                    .Ret();

                result = result.WithGetter(getMethod);
            }

            if (hasSet)
            {
                var setMethod = tb.DefineMethod($"__set_{name}", true, false, interfaceImpl);
                var par = setMethod.DefineParameter("value", type, false, false);
                setMethod.Body.LdArg_0().LdArg(par).StFld(backingField);

                result = result.WithSetter(setMethod);
            }

            return result;
        }

        public static (RoslynPropertyBuilder prop, RoslynField field, RoslynMethodBuilder getMethod, RoslynMethodBuilder setMethod)
            DefineProperty(this RoslynTypeBuilder tb, RoslynType type, string name, bool hasGet, bool hasSet,
                bool interfaceImpl,
                RoslynProperty overrideProperty = null)
        {
            var backingField = tb.DefineField(type, ConventionsHelper.GetBackingFieldName(name), false, false);

            RoslynMethodBuilder getMethod = null, setMethod = null;

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

        public static RoslynPropertyBuilder DefinePropertyWithBackingField(this RoslynTypeBuilder tb, RoslynType type,
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
        public static RoslynConstructorBuilder DefineDefaultConstructor(this RoslynTypeBuilder tb, bool isStatic)
        {
            var c = tb.DefineConstructor(isStatic);
            // if (!isStatic)
            //     c.Body.LdArg_0().EmitCall(tb.BaseType.Constructors[0]).Ret();

            return c;
        }
    }
}