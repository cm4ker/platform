using System;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua
{
    public static class PlatformTypeSystemExtensions
    {
        public static IBackendObject GetBackendObject(this IPUniqueObject type)
        {
            return type.TypeManager.BackendObjects.FirstOrDefault(x => x.ParentId == type.Id) ??
                   throw new Exception("Backend object for this not found");
        }

        public static IType FindType(IPType type)
        {
            return (IType) type.GetBackendObject().PinnedObject;
        }

        public static IType FindType(this ITypeManager tm, string ns, string typeName)
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


        public static IType ToBackend(this IPType type)
        {
            return (IType) type.GetBackendObject().PinnedObject;
        }

        public static IConstructor ToBackend(this IPConstructor constructor)
        {
            return (IConstructor) constructor.GetBackendObject().PinnedObject;
        }

        public static IMethod ToBackend(this IPMethod method)
        {
            return (IMethod) method.GetBackendObject().PinnedObject;
        }

        public static IProperty ToBackend(this IPProperty property)
        {
            return (IProperty) property.GetBackendObject().PinnedObject;
        }

        public static IField ToBackend(this IPField property)
        {
            return (IField) property.GetBackendObject().PinnedObject;
        }

        public static IParameter ToBackend(this IPParameter property)
        {
            return (IParameter) property.GetBackendObject().PinnedObject;
        }
    }
}