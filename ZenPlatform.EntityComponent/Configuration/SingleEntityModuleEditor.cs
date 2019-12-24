using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class SingleEntityModuleEditor
    {

        private IXCProgramModule _module;

        public SingleEntityModuleEditor(IXCProgramModule module)
        {
            _module = module;
        }

        public SingleEntityModuleEditor SetText(string text)
        {
            _module.ModuleText = text;
            return this;
        }

        public SingleEntityModuleEditor SetRelationTypeObject()
        {
            _module.ModuleRelationType = XCProgramModuleRelationType.Object;

            return this;
        }


    }
}
