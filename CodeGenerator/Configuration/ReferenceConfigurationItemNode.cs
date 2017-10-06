using System.Collections.Generic;

namespace CodeGeneration.Configuration
{
    public class ReferenceConfigurationItemNode : ConfigurationNode
    {
        public ReferenceConfigurationItemNode() : base()
        {
            Propertyes = new List<CNProperty>();
        }

        public List<CNProperty> Propertyes { get; set; }
        public List<CNTable> Tables { get; set; }
    }
}