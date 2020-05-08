using Aquila.Configuration.Contracts;

namespace Aquila.EntityComponent.Configuration
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


    public class CommandEditor
    {
        private readonly MDCommand _md;

        public CommandEditor(MDCommand md)
        {
            _md = md;
            if (_md.Module == null)
                _md.Module = new MDProgramModule();
        }

        public string Name
        {
            get => _md.Name;
            set => _md.Name = value;
        }

        public string DisplayName
        {
            get => _md.DisplayName;
            set => _md.DisplayName = value;
        }

        public string ModuleText
        {
            get => _md.Module.ModuleText;
            set => _md.Module.ModuleText = value;
        }
    }
}