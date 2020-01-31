namespace ZenPlatform.EntityComponent.Configuration
{
    public class PropertyEditor
    {
        private MDProperty _mp;

        public PropertyEditor()
        {
            _mp = new MDProperty();
        }

        public string Name
        {
            get => _mp.Name;
            set => _mp.Name = value;
        }

        public void Apply()
        {
            //TODO: Register property
        }
    }
}