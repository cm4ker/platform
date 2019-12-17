using System.ComponentModel;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    /// <summary>
    /// Описывает ссылку как часть метаданных
    /// </summary>
    public interface IXCLinkType : IXCObjectReadOnlyType, IChildItem<IXCComponent>
    {
        IXCObjectType ParentType { get; }
    }
}