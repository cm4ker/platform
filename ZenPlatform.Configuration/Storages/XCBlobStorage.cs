using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Core.Crypto;
using ZenPlatform.Data;
using ZenPlatform.Shared.ParenChildCollection;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration
{

    public interface IDataStorage
    {
        Uri Save(Uri uri, byte[] data);

        byte[] Load(Uri uri);

        bool CanUse(Uri uri);
    }


    public static class DataStorageExtension
    {

        public static void Save(this IDataStorage storage, string uri, byte[] data)
        {
           storage.Save(new Uri(uri), data);
        }

    }



    public class MemoryDataStorage: IDataStorage
    {
        public Dictionary<Uri, byte[]> Blobs { get; }
        public MemoryDataStorage()
        {
            Blobs = new Dictionary<Uri, byte[]>();
        }

        public bool CanUse(Uri uri)
        {
            return string.IsNullOrEmpty(uri.Scheme)
                && uri.Scheme == "memory";
        }

        public Uri Save(Uri uri, byte[] data)
        {
            if (Blobs.ContainsKey(uri))
            {
                Blobs[uri] = data;
            }
            Blobs.Add(uri, data);

            return uri;
        }

        public byte[] Load(Uri uri)
        {
            if (Blobs.ContainsKey(uri))
            {
                return Blobs[uri];
            }
            throw new NotSupportedException("Unable to find data.");
        }
    }

    public class DatabaseDataStorage : IDataStorage
    {
        private IDataContextManager _dataContextManager;

        public DatabaseDataStorage(IDataContextManager dataContextManager)
        {
            _dataContextManager = dataContextManager;
        }

        public bool CanUse(Uri uri)
        {
            return string.IsNullOrEmpty(uri.Scheme)
                && uri.Scheme == "db";
        }

        public byte[] Load(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Uri Save(Uri uri, byte[] data)
        {
            throw new NotImplementedException();
        }
    }


    public class FileDataStorage : IDataStorage
    {
        public bool CanUse(Uri uri)
        {
            return string.IsNullOrEmpty(uri.Scheme)
                && uri.Scheme == "file";
        }

        public byte[] Load(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Uri Save(Uri uri, byte[] data)
        {
            throw new NotImplementedException();
        }
    }


    public interface IConfigurationKeeper
    {
        void Save(IXCSaveable data);

        byte[] Load(IXCBlob blob);
    }

    public class MemoryConfigurationKeeper : IConfigurationKeeper
    {
        public MemoryDataStorage _storage = new MemoryDataStorage();
        public byte[] Load(IXCBlob blob)
        {
            return _storage.Load(blob.URI);
        }

        public void Save(IXCSaveable data)
        {
            _storage.Save(data.GetBlob().URI, data.GetData());

            data.SetHash(HashHelper.HashMD5(data.GetData()));
        }
    }

    public class DatabaseConfigurationKeeper : ConfigurationKeeper
    {
        public DatabaseConfigurationKeeper(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<DatabaseDataStorage>(), 
                  serviceProvider.GetRequiredService<FileDataStorage>(), 
                  serviceProvider.GetRequiredService<MemoryDataStorage>())
        {

        }
    }

    public class FileConfigurationKeeper : ConfigurationKeeper
    {
        public FileConfigurationKeeper(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<FileDataStorage>(),
                  serviceProvider.GetRequiredService<DatabaseDataStorage>(),
                  serviceProvider.GetRequiredService<MemoryDataStorage>())
        {

        }
    }

    public class ConfigurationKeeper: IConfigurationKeeper
    {
        private List<IDataStorage> _storages;
        private IDataStorage _defaultStorage;
        public ConfigurationKeeper(IDataStorage defaultStorage, params IDataStorage[] storages)
        {

            _storages = new List<IDataStorage>();
            _defaultStorage = defaultStorage;
            _storages.Add(defaultStorage);
            if (storages != null)
            {
                _storages.AddRange(storages);
            } 
        }
        public void Save(IXCSaveable data)
        {

            if (_defaultStorage.CanUse(data.GetBlob().URI))
            {
                _defaultStorage.Save(data.GetBlob().URI, data.GetData());
                data.SetHash(HashHelper.HashMD5(data.GetData()));
            }

        }

        public byte[] Load(IXCBlob blob)
        {
            return _storages.First(s => s.CanUse(blob.URI)).Load(blob.URI);
        }
    }



}
