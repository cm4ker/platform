using System.Collections.Generic;
using SqlPlusDbSync.Configuration.Configuration;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class SRelation
    {
        public PType Type { get; set; }
        public List<SCondition> Condition { get; set; }
        public SRelationType RelationType { get; set; }
    }


    public enum SRelationType
    {
        OneToOne,
        OneToMany
    }
}