using System;

namespace Aquila.Core
{
    /// <summary>
    /// Describes to the platform that this is field for query store
    /// </summary>
    public class QueryAttribute : Attribute
    {
    }

    /// <summary>
    /// Describes entity generated type
    /// </summary>
    public class EntityAttribute : Attribute
    {
    }

    /// <summary>
    /// Describes entity link generated type
    /// </summary>
    public class LinkAttribute : Attribute
    {
    }

    public class TargetPlatform : Attribute
    {
        public string Version { get; }

        public TargetPlatform(string version)
        {
            Version = version;
        }
    }

    /// <summary>
    /// Marks the method as extension. This method will be visible in the global context 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtensionAqAttribute : Attribute
    {
    }


    /// <summary>
    /// Http handler attribute
    /// </summary>
    public class HttpHandlerAttribute : Attribute
    {
        public HttpMethodKind Kind { get; }
        public string Route { get; }

        public HttpHandlerAttribute(HttpMethodKind kind, string route)
        {
            Kind = kind;
            Route = route;
        }
    }


    public class CrudHandlerAttribute : Attribute
    {
        public CrudHandlerAttribute(HttpMethodKind kind, string objectName)
        {
            Kind = kind;
            ObjectName = objectName;
        }

        public HttpMethodKind Kind { get; }

        public string ObjectName { get; }
    }

    public enum HttpMethodKind
    {
        Get = 0,
        Post = 1,
        Delete = 2,
        Create = 3,
        List = 4,
    }

    /// <summary>
    /// This action for save entity 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveEntityMethodAttribute : Attribute
    {
    }

    /// <summary>
    /// This action for delete entity 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RemoveEntityMethodAttribute : Attribute
    {
    }

    public class RuntimeInitAttribute : Attribute
    {
        public RuntimeInitKind Kind { get; }
        public object[] Parameters { get; }


        public RuntimeInitAttribute(RuntimeInitKind kind) : this(kind, new object[] { })
        {
        }

        public RuntimeInitAttribute(RuntimeInitKind kind, object param) : this(kind, new object[] { param })
        {
        }

        public RuntimeInitAttribute(RuntimeInitKind kind, object param1, object param2) : this(kind,
            new object[] { param1, param2 })
        {
        }

        public RuntimeInitAttribute(RuntimeInitKind kind, params object[] parameters)
        {
            Kind = kind;
            Parameters = parameters;
        }
    }

    public enum RuntimeInitKind
    {
        TypeId = 0,

        SelectQuery = 1,
        UpdateQuery = 2,
        InsertQuery = 3,
    }
}