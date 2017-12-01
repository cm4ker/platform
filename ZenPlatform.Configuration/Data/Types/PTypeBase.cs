using System;

namespace ZenPlatform.Configuration.Data
{
    public abstract class PTypeBase
    {
        protected PTypeBase()
        {

        }

        public abstract Guid Id { get; }

        public virtual string Name { get; }

        public virtual string Description { get; set; }
    }
}