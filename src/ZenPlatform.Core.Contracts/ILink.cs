using System;

namespace ZenPlatform.Core.Contracts
{
    public interface ILink
    {
        Guid Id { get; }
        int TypeId { get; }
        string Presentation { get; }
    }
}