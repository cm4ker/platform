using System;
using System.IO;
using ZenPlatform.Configuration.Contracts;
using System.Linq;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.Configuration
{
    public class XCConfManipulator : IConfigurationManipulator
    {
        public IProject Load(IXCConfigurationStorage storage)
        {
            return Structure.Project.Load(storage);
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
            var storage1 = new XCMemoryStorage();

            var storage2 = new XCMemoryStorage();

            a.Save(storage1);
            b.Save(storage2);

            if (storage1.Blobs.Count != storage2.Blobs.Count) return false;

            var join = storage1.Blobs.Join(storage2.Blobs, k => k.Key, k => k.Key, (l, r) => new {left = l, right = r});

            foreach (var item in join)
            {
                if (!item.left.Value.SequenceEqual(item.right.Value)) return false;
            }

            return true;
        }
    }
}