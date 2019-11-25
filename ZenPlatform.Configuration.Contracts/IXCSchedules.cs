using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSchedules
    {
        List<IXCFile> IncludedFiles { get; set; }
    }
}