using System;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IField : IEquatable<IField>, IMember
    {
        IType FieldType { get; }

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