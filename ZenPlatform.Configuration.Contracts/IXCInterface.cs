using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCInterface
    {
        List<IXCFile> IncludedFiles { get; set; }
    }
}