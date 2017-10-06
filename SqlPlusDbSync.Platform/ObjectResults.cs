using System;
using System.Xml;
using SqlPlusDbSync.Platform.Configuration;

namespace SqlPlusDbSync.Platform
{
    public class ObjectResults
    {
        public ObjectResults(SType sType, Guid id, byte[] version)
        {
            SType = sType;
            Id = id;
            Version = version;
        }

        public SType SType { get; set; }
        public Guid Id { get; set; }
        
        public byte[] Version { get; set; }
    }
}