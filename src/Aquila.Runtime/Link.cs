using System;
using Aquila.Core.Contracts;

namespace Aquila.Core
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

        public Guid Id => _linkId;

        public int TypeId => _typeId;

        public string Presentation => _presentation;
    }
}