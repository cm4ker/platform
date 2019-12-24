using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Structure;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class SingleEntityCommandEditor
    {
        private XCCommand _command;
        public SingleEntityCommandEditor(XCCommand command)
        {
            _command = command;
        }

        public SingleEntityCommandEditor SetGuid(Guid guid)
        {
            _command.Guid = guid;
            return this;
        }
        public SingleEntityCommandEditor SetName(string name)
        {
            _command.Name = name;
            return this;
        }

        public SingleEntityCommandEditor SetDisplayName(string name)
        {
            _command.DisplayName = name;
            return this;
        }

        public SingleEntityModuleEditor EditModule()
        {
            return new SingleEntityModuleEditor(_command.Module);
        }



    }
}
