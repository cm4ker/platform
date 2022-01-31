using System.Collections.Generic;
using Aquila.Core.Querying;


namespace Aquila.Core
{
    public abstract class AqReader
    {
        /// <summary>
        /// Indexer for access items via associate array
        /// </summary>
        /// <param name="value"></param>
        public abstract object this[string value] { get; }

        public abstract bool read();
    }
}