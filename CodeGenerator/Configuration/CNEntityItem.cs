using System;
using System.Collections.Generic;

namespace CodeGeneration.Configuration
{
    public class CNEntityItem : ConfigurationNode
    {
        private CNEntityItem()
        {
            TableName = "Entity_" + Guid.NewGuid().ToString().Substring(0, 4);
        }

        public CNEntityItem(CNEntity parent) : this()
        {
            Parent = parent;
            parent.Childs.Add(this);

            Propertyes = new List<CNProperty>();
            Tables = new List<CNTable>();
        }

        public string TableName { get; set; }

        public List<CNProperty> Propertyes { get; set; }
        public List<CNTable> Tables { get; set; }

    }
}