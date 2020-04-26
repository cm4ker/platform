using System;
using System.IO;
using SharpFileSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration.Storage
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