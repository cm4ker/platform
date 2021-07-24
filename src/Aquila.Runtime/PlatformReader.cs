using System.Collections.Generic;
using Aquila.Core.Querying;


namespace Aquila.Data
{
    public abstract class PlatformReader
    {
        public virtual object this[string value]
        {
            get { return null; }
        }

        public virtual bool Read()
        {
            return false;
        }
    }
}