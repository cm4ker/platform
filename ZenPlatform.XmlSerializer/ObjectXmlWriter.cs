using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace ZenPlatform.XmlSerializer
{
    /// <summary>
    /// Внутренний класс для обработки объекта в xml представление
    /// </summary>
    internal class ObjectXmlWriter
    {
        private readonly object _instance;
        private readonly XmlWriter _xmlWriter;
        private readonly Type _type;
        private string _typeName;
        private readonly SerializerConfiguration _conf;
        private bool _isCollection;
        private InstanceType _instanceType;

        public ObjectXmlWriter(object instance, XmlWriter xmlWriter)
        {
            _instance = instance;
            _xmlWriter = xmlWriter;
            _type = instance.GetType();
            _conf = SerializerConfiguration.Create();
            _isCollection = (instance is ICollection);
            _instanceType = GetInstanceType(instance);
            _typeName = GetNameWithoutGenericArity(_type);
        }

        public ObjectXmlWriter(object instance, XmlWriter xmlWriter, SerializerConfiguration configuration) : this(
            instance, xmlWriter)
        {
            _conf = configuration;
        }

        public void Handle()
        {
            if (!_isCollection)
            {
                //Поведение по умолчанию: просто интерпретируем тип как имя элемента
                _xmlWriter.WriteStartElement(_typeName);
                if (_instanceType == InstanceType.Primitive)
                {
                    HandleValue(_instance);
                }
                else
                {
                    HandleFields();
                    HandleProperties();
                }

                _xmlWriter.WriteEndElement();
            }
            else
            {
                var collection = _instance as ICollection;
                foreach (var item in collection)
                {
                    new ObjectXmlWriter(item, _xmlWriter, _conf).Handle();
                }
            }
        }

        #region Field handling

        private void HandleFields()
        {
            foreach (var field in _type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = field.GetValue(_instance);
                HandleObjectField(field.Name, value);
            }
        }

        #endregion

        #region Property handling

        private void HandleProperties()
        {
            foreach (var prop in _type.GetProperties())
            {
                if (_conf.IgnoreProperties.Contains(prop)) continue;

                var value = prop.GetValue(_instance);
                HandleObjectField(prop.Name, value);
            }
        }

        #endregion

        private void HandleObjectField(string name, object value)
        {
            _xmlWriter.WriteStartElement(name);
            HandleValue(value);
            _xmlWriter.WriteEndElement();
        }

        private void HandleValue(object value)
        {
            if (value == null) return;
            var valueType = GetInstanceType(value);
            switch (valueType)
            {
                case InstanceType.Primitive:
                    _xmlWriter.WriteValue(value);
                    break;
                default:
                    new ObjectXmlWriter(value, _xmlWriter).Handle();
                    break;
            }
        }

        private InstanceType GetInstanceType(object value)
        {
            switch (value)
            {
                case bool _:
                case byte _:
                case byte[] _:
                case DateTime _:
                case DateTimeOffset _:
                case decimal _:
                case double _:
                case short _:
                case int _:
                case long _:
                case sbyte _:
                case float _:
                case string _:
                case TimeSpan _:
                case ushort _:
                case uint _:
                case ulong _:
                    return InstanceType.Primitive;
                    break;
                default:
                    return InstanceType.Object;
                    break;
            }
        }

        public static string GetNameWithoutGenericArity(Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }

    /// <summary>
    /// Внутренний класс для обработки xml-контента в объект 
    /// </summary>
    internal class XmlObjectWriter
    {
        private readonly Type _targetType;
        private readonly XmlReader _reader;
        private readonly SerializerConfiguration _conf;
        private object _result;
        
        public XmlObjectWriter(Type targetType, XmlReader reader, SerializerConfiguration configuration)
        {
            _targetType = targetType;
            _reader = reader;
            _conf = configuration;
        }

        public void Handle()
        {
        }

        public object Result => _result;
    }
}