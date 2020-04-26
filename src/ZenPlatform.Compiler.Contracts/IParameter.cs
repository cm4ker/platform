using System;

namespace ZenPlatform.Compiler.Contracts
{
    /// <summary>
    /// Contract for the method parameter
    /// </summary>
    public interface IParameter : IEquatable<IParameter>
    {
        /// <summary>
        /// Parameter name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parameter type
        /// </summary>
        IType Type { get; }

        int Sequence { get; }

        int ArgIndex { get; }
    }
}