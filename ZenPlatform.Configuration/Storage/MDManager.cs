﻿using System;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.Configuration.Storage
{
    public class MDManager : ILoader, IXCSaver
    {
        private IXCConfigurationStorage _storage;
        private ITypeManager _typeManager;

        public MDManager(IXCConfigurationStorage storage, ITypeManager typeManager)
        {
            _storage = storage;
            _typeManager = typeManager;
        }


        public IXCConfigurationStorage Storage => _storage;

        public IUniqueCounter Counter => _storage;

        public ISettingsManager Settings => _storage;

        public ITypeManager TypeManager => _typeManager;

        public T LoadObject<T>(string path)
        {
            try
            {
                using (var stream = _storage.GetBlob("", path))
                {
                    return XCHelper.DeserializeFromStream<T>(stream);
                }
            }
            catch
            {
                return default;
            }
        }

        public byte[] LoadBytes(string path)
        {
            using (var stream = _storage.GetBlob("", path))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                byte[] myBynary = memoryStream.ToArray();
                return memoryStream.ToArray();
            }
        }

        public void SaveObject(string path, object obj)
        {
            _storage.SaveBlob("", path, obj.SerializeToStream());
        }

        public void SaveBytes(string path, byte[] data)
        {
            _storage.SaveBlob("", path, new MemoryStream(data));
        }
    }
}