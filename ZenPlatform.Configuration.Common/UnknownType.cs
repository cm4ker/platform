using System;

namespace ZenPlatform.Configuration.Common
{
    public class MDPrimitive : MDType
    {
    }

    /// <summary>
    /// Неопределённый тип, при загрузке конфигурации сначала всё приводится к нему
    /// </summary>
    public class UnknownType : MDType
    {
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

    /// <summary>
    /// Неопределённый тип, при загрузке конфигурации сначала всё приводится к нему
    /// </summary>
    public sealed class TypeRef : MDType
    {
        public TypeRef()
        {
        }

        public TypeRef(Guid guid)
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