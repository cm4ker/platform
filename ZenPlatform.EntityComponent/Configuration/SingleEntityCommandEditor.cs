using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class SingleEntityCommandEditor : ICommandEditor
    {
        private XCCommand _command;

        public SingleEntityCommandEditor(XCCommand command)
        {
            _command = command;
        }

        public ICommandEditor SetGuid(Guid guid)
        {
            _command.Guid = guid;
            return this;
        }
        public ICommandEditor SetName(string name)
        {
            _command.Name = name;
            return this;
        }

        public ICommandEditor SetDisplayName(string name)
        {
            _command.DisplayName = name;
            return this;
        }

        public IModuleEditor EditModule()
        {
            return new SingleEntityModuleEditor(_command.Module);
        }
    }
}