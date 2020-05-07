using System;

namespace ZenPlatform.Configuration.Common
{
    /// <summary>
    /// Неопределённый тип, при загрузке конфигурации сначала всё приводится к нему
    /// </summary>
    public sealed class MDTypeRef : MDType
    {
        public MDTypeRef()
        {
        }

        public MDTypeRef(Guid guid)
        {
            Guid = guid;
        }

        protected override bool ShouldSerializeDescription()
        {
            return false;
        }

        protected override bool ShouldSerializeName()
        {
            return false;
        }

        protected override bool ShouldSerializeId()
        {
            return false;
        }
    }
}