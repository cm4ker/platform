using System;
using System.IO;
using SharpFileSystem;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.Store;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Configuration.Structure;

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