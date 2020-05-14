using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Common.TypeSystem
{
    /// <summary>
    /// This is for configuration methods description this is not a CLR method
    /// </summary>
    public class PMethod : IPMethod
    {
        private readonly TypeManager _tm;

        internal PMethod(TypeManager tm)
        {
            _tm = tm;
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }
        public IPType ReturnType { get; }

        public ITypeManager TypeManager => _tm;
    }
}