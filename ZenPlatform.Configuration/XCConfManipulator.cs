using System;
using System.Collections.Generic;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using System.Linq;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration
{
    public class XCConfManipulator : IConfigurationManipulator
    {
        public IProject Load(IFileSystem storage)
        {
            return null;
        }

        public IProject Create(string projectName)
        {
            return null;
        }

        public Stream SaveToStream(IProject project)
        {
            return project.SerializeToStream();
        }

        public string GetHash(IProject project)
        {
            return ((Project) project).GetHash();
        }

        public bool Equals(IProject a, IProject b)
        {
            throw new NotImplementedException();
        }
    }


    public class InMemoryUniqueCounter : IUniqueCounter
    {
        private Dictionary<Guid, uint> _dic;
        private uint _maxId = 100;


        public InMemoryUniqueCounter()
        {
            _dic = new Dictionary<Guid, uint>();
        }

        public uint GetId(Guid confId)
        {
            if (_dic.TryGetValue(confId, out var a))
                return a;
            else
            {
                var val = _maxId++;
                _dic.Add(confId, val);
                return val;
            }
        }
    }
}