using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace ZenPlatform.Compiler.Contracts
{
    public interface ICustomAttribute : IEquatable<ICustomAttribute>
    {
        List<object> Parameters { get; }
        Dictionary<string, object> Properties { get; }
    }


    public interface ICustomAttributeBuilder : ICustomAttribute
    {
        void SetParameters(params object[] args);

        void SetNamedProperties(Dictionary<string, object> props);
    }
}