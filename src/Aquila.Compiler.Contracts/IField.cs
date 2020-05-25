using System;

namespace Aquila.Compiler.Contracts
{
    public interface IField : IEquatable<IField>, IMember
    {
        IType FieldType { get; }

        IType DeclaringType { get; }

        bool IsPublic { get; }

        bool IsStatic { get; }

        bool IsLiteral { get; }

        object GetLiteralValue();
    }

    public interface IFieldBuilder : IField
    {
        void SetAttribute(ICustomAttribute attr);
    }
}