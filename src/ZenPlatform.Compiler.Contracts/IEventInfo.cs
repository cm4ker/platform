using System;

namespace ZenPlatform.Compiler.Contracts
{
    public interface IEventInfo : IEquatable<IEventInfo>, IMember
    {
        IMethod Add { get; }
    }
}