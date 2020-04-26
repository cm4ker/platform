using System;
using System.Collections.Generic;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Environment;

namespace ZenPlatform.Core
{
    public class LinkFactory : ILinkFactory
    {
        private Dictionary<int, Func<Guid, string, ILink>>
            _factories = new Dictionary<int, Func<Guid, string, ILink>>();

        public void Register(int typeId, Func<Guid, string, ILink> facDelegate)
        {
            if (!_factories.ContainsKey(typeId))
                _factories.Add(typeId, facDelegate);
        }

        public ILink Create(int typeId, Guid linkId, string presentation)
        {
            return _factories[typeId](linkId, presentation);
        }
    }
}