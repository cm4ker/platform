using Aquila.Component.Shared.Configuration;

namespace Aquila.EntityComponent.Configuration.Editors
{
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