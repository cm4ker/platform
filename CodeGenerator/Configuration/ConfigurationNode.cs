using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CodeGeneration.Configuration
{

    public abstract class ConfigurationNode
    {
        protected ConfigurationNode()
        {
            Id = Guid.NewGuid();
            Childs = new List<ConfigurationNode>();
        }

        public List<ConfigurationNode> Childs { get; set; }
        [JsonIgnore]
        public ConfigurationNode Parent { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }

        public List<CNEntity> GetEntityes()
        {
            return Childs.OfType<CNEntity>().ToList();
        }

        public List<CNDifference> Compare(ConfigurationNode node)
        {
            var result = new List<CNDifference>();

            if (node.Id != this.Id) return new List<CNDifference> { new CNDifference() { Action = CNAction.Add } };
            CNHash hashNode1 = new CNHash(this);
            CNHash hashNode2 = new CNHash(node);
            if (hashNode2 == hashNode1) return result;

            var nodeChilds = node.Childs.ToList();

            foreach (var child in Childs)
            {
                var nodeChild = node.Childs.FirstOrDefault(x => x.Id == child.Id);
                if (nodeChild != null)
                {
                    result.AddRange(child.Compare(nodeChild));
                    nodeChilds.Remove(nodeChild);
                }
                result.Add(new CNDifference() { Action = CNAction.Remove, Node = child });
            }

            foreach (var child in nodeChilds)
            {
                result.Add(new CNDifference() { Action = CNAction.Add, Node = child });
            }

            return result;
        }

    }
}
