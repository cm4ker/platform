using System;
using System.Collections.Generic;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDTable
    {
        public MDTable()
        {
            
        }
        
        public virtual Guid Guid { get; set; }

        public virtual string Name { get; set; }

        public List<MDProperty> Properties { get; set; }
    }
}