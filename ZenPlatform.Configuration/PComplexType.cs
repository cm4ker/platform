namespace SqlPlusDbSync.Configuration
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
            Propertyes.AddRange(_objectType.Propertyes);
        }
    }
}