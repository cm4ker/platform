using System;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPropertyType : ITypeManagerProvider
    {
        Guid PropertyParentId { get; set; }
        Guid PropertyId { get; set; }
        Guid TypeId { get; set; }
    }
}