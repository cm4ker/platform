using System;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IPTypeSpec : IPType , IEquatable<IPType>
    {
        int Scale { get; set; }
        int Precision { get; set; }
        int Size { get; set; }
    }
}