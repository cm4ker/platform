using System;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using System.Linq;
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

        public bool Equals(IXCRoot a, IXCRoot b)
        {
            var storage1 = new XCMemoryStorage();

            var storage2 = new XCMemoryStorage();

            a.Save(storage1);
            b.Save(storage2);

            if (storage1.Blobs.Count != storage2.Blobs.Count) return false;

            var join = storage1.Blobs.Join(storage2.Blobs, k => k.Key, k => k.Key, (l, r) => new { left = l, right = r });

            foreach (var item in join)
            {
                if (!item.left.Value.SequenceEqual(item.right.Value)) return false;

            }

            return true;
        }
    }
}