using System;

namespace Aquila.Core
{
    /// <summary>
    /// Unique counter. Returns next id for destination point
    /// </summary>
    public interface IUniqueCounter
    {
        /// <summary>
        /// Returns the identifier by the metadata name
        /// </summary>
        uint GetId(string metadataKey);
    }
}