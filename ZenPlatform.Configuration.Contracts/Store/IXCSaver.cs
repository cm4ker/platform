using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCSaver
    {
        void SaveObject<T>(string path, IMetaDataItem<T> item)
            where
            T : IMDItem;

        void SaveBytes(string path, byte[] data);

        ITypeManager TypeManager { get; }
    }
}