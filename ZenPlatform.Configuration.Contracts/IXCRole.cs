using System;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCRole : IChildItem<IXCRoles>
    {
        Guid Id { get; set; }
        string Name { get; set; }
        IXCPlatformRule PlatformRules { get; set; }
        ChildItemCollection<IXCRole, IXCDataRuleContent> DataRules { get; }
        IXCRoles Roles { get; }
        IXCRoot Root { get; }
    }
}