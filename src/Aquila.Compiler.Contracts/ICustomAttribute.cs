using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Aquila.Compiler.Contracts
{
    public interface ICustomAttribute : IEquatable<ICustomAttribute>
    {
        IType AttributeType { get; }

        List<object> Parameters { get; }
        Dictionary<string, object> Properties { get; }
    }


    public interface ICustomAttributeBuilder : ICustomAttribute
    {
        void SetParameters(params object[] args);

        void SetNamedProperties(Dictionary<string, object> props);
    }
}