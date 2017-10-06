using System.Collections.Generic;

namespace CodeGeneration.Configuration
{
    public class RapidEntityConfiguratironItemNode : ConfigurationNode
    {
        public RapidEntityConfiguratironItemNode(RapidEntityConfigurationNode parent)
        {
            Parent = parent;
            parent.Childs.Add(this);

            Propertyes = new List<CNProperty>();
            Indexes = new List<IndexesConfigurationItemNode>();
            Constraints = new List<ConstraintConfigurationItemNode>();
        }

        public List<CNProperty> Propertyes { get; set; }
        public List<IndexesConfigurationItemNode> Indexes { get; set; }
        public List<ConstraintConfigurationItemNode> Constraints { get; set; }
    }
}