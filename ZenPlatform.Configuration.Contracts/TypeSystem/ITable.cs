using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITable : ITypeManagerProvider
    {
        Guid Id { get; set; }
        
        Guid GroupId { get; set; }
        
        string Name { get; set; }
        
        Guid ParentId { get; set; }
        
        IEnumerable<IPProperty> Properties { get; }
    }
}