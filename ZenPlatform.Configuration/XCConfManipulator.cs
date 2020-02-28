﻿using System;
using System.Collections.Generic;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using System.Linq;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Crypto;

namespace ZenPlatform.Configuration
{
    public class XCConfManipulator : IConfigurationManipulator
    {
        public IProject Load(IFileSystem storage)
        {
            return Project.Load(new MDManager(new TypeManager(), new InMemoryUniqueCounter()), storage);
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
            //TODO: This is hack for now. Need make tihs more essential
            var rnd = new Random((int) DateTime.Now.Ticks);
            var buffer = new byte[1024];
            rnd.NextBytes(buffer);


            return "ConfigurationMD5"; // HashHelper.HashMD5(buffer);
        }

        public bool Equals(IProject a, IProject b)
        {
            return false;
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