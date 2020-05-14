using System;
using System.IO;
using SharpFileSystem;
using Aquila.Configuration.Structure;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Configuration.Storage
{
    public class MDManager : IInfrastructure
    {
        private IFileSystem _storage;

        public MDManager(ITypeManager typeManager, IUniqueCounter counter)
        {
            TypeManager = typeManager;
            Counter = counter;
        }

        public IUniqueCounter Counter { get; }

        public ISettingsManager Settings => null;

        public ITypeManager TypeManager { get; }
    }
}