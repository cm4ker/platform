namespace ZenPlatform.Configuration.Data
{
    /// <summary>
    /// Комплексный тип данных
    /// На этом уровне появляется привязка к событиям
    /// </summary>
    public class PComplexType : PObjectType
    {
        private readonly PObjectType _objectType;

        protected PComplexType(string name, PObjectType objectType) : base(name)
        {
            _objectType = objectType;
            Properties.AddRange(_objectType.Properties);
        }

        public PObjectType Owner => _objectType;
    }
}