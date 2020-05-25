using System;
using System.Linq;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Core.Network;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace Aquila.Compiler
{
    public class PlatformRealWorldProvider
    {
        public PlatformRealWorldProvider()
        {
        }
    }

    public static class PlatformTypeSystemExtensions
    {
        public static IBackendObject GetBackendObject(this IPUniqueObject type)
        {
            return type.TypeManager.BackendObjects.FirstOrDefault(x => x.ParentId == type.Id) ??
                   throw new Exception("Backend object for this not found");
        }

        public static RoslynType FindType(IPType type)
        {
            return (RoslynType) type.GetBackendObject().PinnedObject;
        }

        public static RoslynType FindType(this ITypeManager tm, string ns, string typeName)
        {
            var result = tm.Types.Where(x => x.Name == typeName && x.GetNamespace() == ns).ToList();

            if (result.Count > 1)
                throw new Exception("Ambiguous types");
            if (result.Count == 0)
            {
                //TODO: try get type from external type system. For this we need access to provider
                return null;
            }

            return result.First().ToBackend();
        }


        public static RoslynType ToBackend(this IPType type)
        {
            return (RoslynType) type.GetBackendObject().PinnedObject;
        }

        public static RoslynConstructor ToBackend(this IPConstructor constructor)
        {
            return (RoslynConstructor) constructor.GetBackendObject().PinnedObject;
        }

        public static RoslynMethod ToBackend(this IPMethod method)
        {
            return (RoslynMethod) method.GetBackendObject().PinnedObject;
        }

        public static RoslynMethod ToBackend(this IPProperty property)
        {
            return (RoslynMethod) property.GetBackendObject().PinnedObject;
        }

        public static RoslynMethod ToBackend(this IPField property)
        {
            return (RoslynMethod) property.GetBackendObject().PinnedObject;
        }
    }

    public static class BlockExtensions
    {
        public static RBlockBuilder IsInst(this RBlockBuilder bb, IPType type)
        {
            return bb.IsInst(type.ToBackend());
        }

        public static RBlockBuilder NewObj(this RBlockBuilder bb, IPConstructor constructor)
        {
            return bb.NewObj(constructor.ToBackend());
        }

        public static RoslynType GetClrPrivateType(IPType type)
        {
        }
    }


    public class BackendTypeProvider
    {
        private readonly RoslynTypeSystem _ts;

        public BackendTypeProvider(RoslynTypeSystem ts)
        {
            _ts = ts;
        }

        public object FindType(string @namespace, string name)
        {
            return FindType($"{@namespace}.{name}");
        }

        public RoslynType FindType(string fullName)
        {
            return _ts.FindType(fullName);
        }
    }
}