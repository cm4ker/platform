using System;
using System.Xml;
using SqlPlusDbSync.Configuration.Configuration;
using SqlPlusDbSync.Platform.Configuration;

namespace SqlPlusDbSync.Platform
{
    public class ObjectResults
    {
        public ObjectResults(PType pType, Guid id, byte[] version)
        {
            PType = pType;
            Id = id;
            Version = version;
        }

        public PType PType { get; set; }
        public Guid Id { get; set; }
        
        public byte[] Version { get; set; }
    }
}