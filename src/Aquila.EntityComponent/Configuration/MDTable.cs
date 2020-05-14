using System;
using System.Collections.Generic;

namespace Aquila.EntityComponent.Configuration
{
    public class MDTable
    {
        public MDTable()
        {
            Properties = new List<MDProperty>();
            Guid = Guid.NewGuid();
        }

        public Guid Guid { get; set; }

        public string Name { get; set; }

        public List<MDProperty> Properties { get; set; }
    }
}