using System.Collections.Generic;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class XCSingleEntityMetadata : IXCTypeMetadata
    {
        public bool IsAbstract { get; set; }
        
        public bool IsSealed { get; set; }
        
        public IXCType BaseType { get; set; }
        
        public bool HasProperties { get; set; }
        
        public bool HasModules { get; set;}
        
        public bool HasCommands { get; set;}
        
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void LoadDependencies()
        {
            throw new System.NotImplementedException();
        }

        public IXCObjectProperty CreateProperty()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IXCObjectProperty> GetProperties()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IXCProgramModule> GetProgramModules()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IXCCommand> GetCommands()
        {
            throw new System.NotImplementedException();
        }

        public IXCCommand CreateCommand()
        {
            throw new System.NotImplementedException();
        }
    }
}