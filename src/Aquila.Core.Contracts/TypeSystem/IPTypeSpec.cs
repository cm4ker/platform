using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPTypeSpec : IPType, IEquatable<IPType>
    {
        int Scale { get; }
        int Precision { get; }
        int Size { get; }

        void SetScale(int value);
        void SetPrecision(int value);
        void SetSize(int value);
        void SetIsArray(bool value);
    }

    public interface IPTypeSet : IPType, IEquatable<IPType>
    {
        IEnumerable<IPType> Types { get; }

        void AddType(Guid typeId);
    }
}