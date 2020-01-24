using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDEntity : IMDType, IMDItem, IMetaData<MDEntity>
    {
        public MDEntity()
        {
            EntityId = Guid.NewGuid();
            LinkId = Guid.NewGuid();
            ObjectId = Guid.NewGuid();
            DtoId = Guid.NewGuid();
        }

        public List<MDProperty> Properties { get; set; }

        public List<MDTable> Tables { get; set; }

        public string ObjectModule { get; set; }

        public string ManagerModule { get; set; }

        public List<string> TableDefReferences { get; set; }

        public List<MDCommand> Command { get; set; }

        public virtual string RelTableName { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }

        public Guid EntityId { get; set; }

        public Guid LinkId { get; set; }

        public Guid ObjectId { get; set; }

        public Guid DtoId { get; set; }

        public void Initialize(ILoader loader, MDEntity settings)
        {
        }

        public IMDItem Store(IXCSaver saver)
        {
            return this;
        }
    }
}