using System;

namespace ZenPlatform.Core.Contracts
{
    public interface ILinkFactory
    {
        void Register(int typeId, Func<Guid, string, ILink> facDelegate);
        ILink Create(int typeId, Guid linkId, string presentation);
    }
}