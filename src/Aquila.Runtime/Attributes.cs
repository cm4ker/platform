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
}