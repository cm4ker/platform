using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCModules
    {
        /// <summary>
        /// Включенные файлы
        /// </summary>
        List<IXCFile> IncludedFiles { get; set; }
    }
}