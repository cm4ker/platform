using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.Configuration
{
    /// <summary>
    /// Represents class of objects which can materialize in database, wich have unique single identifier and can be determinazed
    /// This objects have properties of objects and lists of objects
    /// For example: cow, reference, etc
    /// </summary>
    public class CNEntity : ConfigurationNode
    {
        private CNEntity()
        {

        }

        public CNEntity(CNRoot parent) : this()
        {
            Parent = parent;
            parent.Childs.Add(this);
        }

        public List<CNEntityItem> EntityItems => Childs.OfType<CNEntityItem>().ToList();



    }
}