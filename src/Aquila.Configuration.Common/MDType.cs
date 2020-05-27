using System;

namespace Aquila.Configuration.Common
{
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
}