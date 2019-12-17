using System;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration
{
    public class XCSimpleCounter : IXCConfigurationUniqueCounter
    {
        private uint _maxId = 100;

        public void GetId(Guid confId, ref uint oldId)
        {
            if (oldId != 0)
                return;

            oldId = _maxId++;
        }
    }

    public class XCConfManipulator : IConfigurationManipulator
    {
        public IXCRoot Load(IXCConfigurationStorage storage)
        {
            return XCRoot.Load(storage);
        }

        public IXCRoot Create(string projectName)
        {
            return XCRoot.Create(projectName);
        }

        public Stream SaveToStream(IXCRoot root)
        {
            return root.SerializeToStream();
        }

        public string GetHash(IXCRoot root)
        {
            return ((XCRoot) root).GetHash();
        }
    }
}