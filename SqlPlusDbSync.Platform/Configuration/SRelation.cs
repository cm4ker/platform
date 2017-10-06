using System.Collections.Generic;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class SRelation
    {
        public SType Type { get; set; }
        public List<SCondition> Condition { get; set; }
        public SRelationType RelationType { get; set; }
    }


    public enum SRelationType
    {
        OneToOne,
        OneToMany
    }
}