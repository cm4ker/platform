using System;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPMethod : ITypeManagerProvider
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Name { get; set; }
        public IPType ReturnType { get; }
    }

    public interface IPArgument
    {
        public string Name { get; set; }

        public IPType Type { get; set; }
    }
}