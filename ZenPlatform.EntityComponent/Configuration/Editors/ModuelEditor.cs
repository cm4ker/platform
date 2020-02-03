using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ModuelEditor
    {
        private readonly MDProgramModule _md;

        public ModuelEditor(MDProgramModule md)
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