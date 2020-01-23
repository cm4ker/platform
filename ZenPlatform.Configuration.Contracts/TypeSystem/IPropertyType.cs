using System;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IPropertyType
    {
        Guid PropertyId { get; set; }
        Guid TypeId { get; set; }
    }
}