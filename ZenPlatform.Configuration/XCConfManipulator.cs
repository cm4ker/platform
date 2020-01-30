using System;
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
            return Project.Load(storage);
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
}