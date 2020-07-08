using System.Collections.Generic;

namespace Aquila.Metadata
{
    public sealed class EntityProperty
    {
        public EntityProperty()
        {
            Types = new List<MetadataType>();
        }

        /// <summary>
        /// Name of property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Types, describes the property
        /// Type example: int, string, date, double 
        /// </summary>
        public List<MetadataType> Types { get; set; }
    }

    public class MetadataType
    {
        /// <summary>
        /// Name of metadata type
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Use for integral values
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Use for floating values
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Use for character values
        /// </summary>
        public int Size { get; set; }
    }
}