using System;

namespace ZenPlatform.Configuration.Data
{
    /// <summary>
    /// Комплексный тип данных
    /// На этом уровне появляется привязка к событиям
    /// </summary>
    public class PComplexType : PObjectType
    {
        private PObjectType _objectType;

        protected PComplexType(string name, PObjectType objectType) : base(name, Guid.Empty, objectType.OwnerComponent)
        {
            Init(objectType);
        }

        protected PComplexType(string name, Guid guid, PObjectType objectType) : base(name, guid, objectType.OwnerComponent)
        {
            Init(objectType);
        }

        private void Init(PObjectType objectType)
        {
            _objectType = objectType;
            Properties.AddRange(_objectType.Properties);
        }

        public PObjectType Owner => _objectType;
    }
}