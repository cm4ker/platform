using System;

namespace Aquila.Core.Contracts
{
    public interface ILink
    {
        Guid Id { get; }
        int TypeId { get; }
        string Presentation { get; }
    }
}