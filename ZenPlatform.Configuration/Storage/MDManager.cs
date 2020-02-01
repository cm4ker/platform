﻿using System;
using System.IO;
using SharpFileSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Storage
{
    public class MDManager : IInfrastructure
    {
        private IFileSystem _storage;

        public MDManager(IFileSystem storage, ITypeManager typeManager)
        {
            _storage = storage;
            TypeManager = typeManager;
        }

        public IUniqueCounter Counter => null;

        public ISettingsManager Settings => null;

        public ITypeManager TypeManager { get; }
    }
}