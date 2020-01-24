using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts.Store;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IMDType
    {
        string RelTableName { get; set; }

        string Name { get; set; }
    }


    public interface IMDProperty
    {
        string DatabaseColumnName { get; set; }
    }

    public interface IMDComponent
    {
        string AssemblyReference { get; set; }
        List<string> EntityReferences { get; set; }
    }
}