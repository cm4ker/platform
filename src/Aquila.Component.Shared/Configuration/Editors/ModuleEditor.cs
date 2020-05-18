using Aquila.Core.Contracts.Configuration;

namespace Aquila.Component.Shared.Configuration.Editors
{
    public class ModuleEditor
    {
        private readonly MDProgramModule _md;

        public ModuleEditor(MDProgramModule md)
        {
            _md = md;
        }

        public string ModuleName
        {
            get => _md.ModuleName;
            set => _md.ModuleName = value;
        }

        public string ModuleText
        {
            get => _md.ModuleText;
            set => _md.ModuleText = value;
        }

        public ProgramModuleDirectionType ModuleDirectionType
        {
            get => _md.ModuleDirectionType;
            set => _md.ModuleDirectionType = value;
        }

        public ProgramModuleRelationType ModuleRelationType
        {
            get => _md.ModuleRelationType;
            set => _md.ModuleRelationType = value;
        }
    }
}