using System;

namespace ZenPlatform.Configuration.Common
{
    public class MDPrimitive : MDType
    {
    }

    public class MDType
    {
        public virtual Guid Guid { get; set; }

        public virtual string Name { get; }

        protected virtual bool ShouldSerializeDescription()
        {
            return false;
        }

        protected virtual bool ShouldSerializeName()
        {
            return false;
        }

        protected virtual bool ShouldSerializeId()
        {
            return false;
        }
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