using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IMDComponent
    {
        List<string> EntityReferences { get; set; }
    }
}