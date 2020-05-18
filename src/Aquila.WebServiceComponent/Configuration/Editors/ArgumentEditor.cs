using Aquila.Configuration.Common;

namespace Aquila.WebServiceComponent.Configuration.Editors
{
    public class ArgumentEditor
    {
        private MDArgument _mp;

        public ArgumentEditor(MDArgument mp)
        {
            _mp = mp;
        }

        public string Name
        {
            get => _mp.Name;
            set => _mp.Name = value;
        }

        public ArgumentEditor SetType(MDType type)
        {
            _mp.Type = type;
            return this;
        }

        public MDType Type => _mp.Type;
    }
}