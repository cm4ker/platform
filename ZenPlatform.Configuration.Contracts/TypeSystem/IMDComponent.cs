using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IMDComponent
    {
        string AssemblyReference { get; set; }
        List<string> EntityReferences { get; set; }
    }
}