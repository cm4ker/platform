using System;
using System.Collections.Generic;

namespace Aquila.Configuration.Storage
{
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
}