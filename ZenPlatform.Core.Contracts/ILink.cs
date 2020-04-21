using System;

namespace ZenPlatform.Core.Contracts
{
    public interface ILink
    {
        Guid LinkId { get; }
        int TypeId { get; }
        string Presentation { get; }
    }
}