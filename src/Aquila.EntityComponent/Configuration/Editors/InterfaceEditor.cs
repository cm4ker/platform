namespace Aquila.EntityComponent.Configuration.Editors
{
    public class InterfaceEditor
    {
        private readonly MDInterface _i;

        public InterfaceEditor(MDInterface i)
        {
            _i = i;
        }


        public string Name
        {
            get => _i.Name;
            set => _i.Name = value;
        }

        public string Markup
        {
            get => _i.Markup;
            set => _i.Markup = value;
        }
    }
}