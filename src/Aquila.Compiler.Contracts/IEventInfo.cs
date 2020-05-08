using System;

namespace Aquila.Compiler.Contracts
{
    public interface IEventInfo : IEquatable<IEventInfo>, IMember
    {
        IMethod Add { get; }
    }
}