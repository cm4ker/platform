using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Common;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDEntity
    {
        public MDEntity()
        {
            EntityId = Guid.NewGuid();
            LinkId = Guid.NewGuid();
            ObjectId = Guid.NewGuid();
            DtoId = Guid.NewGuid();

            Properties = new List<MDProperty>();
            Tables = new List<MDTable>();
            Modules = new List<MDProgramModule>();
            Commands = new List<MDCommand>();
            Interfaces = new List<MDInterface>();
        }

        public string Name { get; set; }

        public Guid ObjectId { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

        public Guid DtoId { get; set; }

        public List<MDProperty> Properties { get; set; }

        public List<MDTable> Tables { get; set; }

        public List<MDCommand> Commands { get; set; }

        public List<MDProgramModule> Modules { get; set; }

        public List<MDInterface> Interfaces { get; set; }
    }
}