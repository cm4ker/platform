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
            return asm.GetTypes().SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttribute<HttpHandlerAttribute>() != null)
                .Select(x => (x, x.GetCustomAttribute<HttpHandlerAttribute>()));
        }


        public static IEnumerable<(MemberInfo m, RuntimeInitAttribute attr)> GetRuntimeInit(this Assembly asm)
        {
            return asm.GetTypes().SelectMany(x => x.GetMembers())
                .Where(x => x.GetCustomAttribute<RuntimeInitAttribute>() != null)
                .Select(x => (x, x.GetCustomAttribute<RuntimeInitAttribute>()));
        }
    }
}