using System;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Environment;

namespace ZenPlatform.Core
{
    /// <summary>
    /// Ссылка 
    /// </summary>
    public class Link : ILink
    {
        private readonly int _typeId;
        private readonly Guid _linkId;
        private readonly string _presentation;

        public Link(int typeId, Guid linkId, string presentation)
        {
            _typeId = typeId;
            _linkId = linkId;
            _presentation = presentation;
        }

        public Guid LinkId => _linkId;

        public int TypeId => _typeId;

        public string Presentation => _presentation;
    }
}