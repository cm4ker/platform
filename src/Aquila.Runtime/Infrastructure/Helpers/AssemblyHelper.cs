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
        public static IEnumerable<MethodInfo> GetLoadMethod(this Assembly asm)
        {
            return asm.GetTypes().SelectMany(x => x.GetMethods())
                .Where(x => x.GetCustomAttribute<HttpHandlerAttribute>() != null);
        }


        public static IEnumerable<(MemberInfo m, RuntimeInitAttribute attr)> GetRuntimeInit(this Assembly asm)
        {
            return asm.GetTypes().SelectMany(x => x.GetMembers())
                .Where(x => x.GetCustomAttribute<RuntimeInitAttribute>() != null)
                .Select(x => (x, x.GetCustomAttribute<RuntimeInitAttribute>()));
        }
    }
}