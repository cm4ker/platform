using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITable : ITypeManagerProvider
    {
        Guid Id { get; set; }
        
        uint SystemId { get; set; }
        
        string Name { get; set; }
        
        Guid ParentId { get; set; }
        
        IEnumerable<IProperty> Properties { get; }
    }
}