using System;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IPropertyType : ITypeManagerProvider
    {
        Guid PropertyId { get; set; }
        Guid TypeId { get; set; }
    }
}