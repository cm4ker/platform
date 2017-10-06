using System;
using System.Collections.Generic;

namespace CodeGeneration.Configuration
{
    public class CNTable : ConfigurationNode
    {

        public CNTable()
        {
            Propertyes = new List<CNProperty>();

            TableName = "Entity_Table_" + Guid.NewGuid().ToString().Substring(0, 4);
        }

        public string TableName { get; set; }
        public List<CNProperty> Propertyes { get; set; }
    }
}