using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aquila.Core;

namespace Aquila.Runtime.Infrastructure.Helpers
{
    public static class AssemblyHelper
    {
        public static IEnumerable<(MethodInfo m, HttpHandlerAttribute attr)> GetLoadMethod(this Assembly asm)
        {
            return GetMembersByAttr<HttpHandlerAttribute, MethodInfo>(asm);
        }

        public static IEnumerable<(MethodInfo m, CrudHandlerAttribute attr)> GetCrudMethods(this Assembly asm)
        {
            return GetMembersByAttr<CrudHandlerAttribute, MethodInfo>(asm);
        }

        public static IEnumerable<(MemberInfo m, RuntimeInitAttribute attr)> GetRuntimeInit(this Assembly asm)
        {
            return GetMembersByAttr<RuntimeInitAttribute>(asm);
        }

        public static IEnumerable<(MethodInfo m, endpoint attr)> GetEndpoints(this Assembly asm)
        {
            return GetMembersByAttr<endpoint, MethodInfo>(asm);
        }

        public static IEnumerable<(MemberInfo m, TAttribute attr)> GetMembersByAttr<TAttribute>(this Assembly asm)
            where TAttribute : Attribute
        {
            return asm.GetTypes().SelectMany(x => x.GetMembers())
                .Where(x => x.GetCustomAttribute<TAttribute>() != null)
                .Select(x => (x, x.GetCustomAttribute<TAttribute>()));
        }

        public static IEnumerable<(TMember m, TAttr attr)> GetMembersByAttr<TAttr, TMember>(this Assembly asm)
            where TAttr : Attribute
            where TMember : MemberInfo
        {
            return asm.GetTypes().SelectMany(x => x.GetMembers().OfType<TMember>())
                .Where(x => x.GetCustomAttribute<TAttr>() != null)
                .Select(x => (x, x.GetCustomAttribute<TAttr>()));
        }
    }
}