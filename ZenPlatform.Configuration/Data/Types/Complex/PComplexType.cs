using System;

namespace ZenPlatform.Configuration.Data.Types.Complex
{
    /// <summary>
    /// Комплексный тип данных
    /// Этот тип данных позволяет осуществлять наследование.
    /// 
    /// Наследование реализуется на уровне компонента и есть ли возможность поддерживать его или нет - решать создателям компонента
    /// 
    /// Допустим у нас есть абстрактный тип "ДокументПрихода". Из этого типа можно создать новые документы:
    ///     - ПриходнаяНакладная
    ///     - ВозвратТовараОтПокупатлея
    /// 
    /// На этом уровне появляется привязка к событиям
    /// </summary>
    public abstract class PComplexType : PObjectType
    {
        private PObjectType _objectType;

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