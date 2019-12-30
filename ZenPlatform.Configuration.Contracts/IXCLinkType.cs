using System.ComponentModel;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Описывает ссылку как часть метаданных
    /// </summary>
    public interface IXCLinkType : IXCObjectReadOnlyType
    {
        IXCObjectType ParentType { get; }
    }
}