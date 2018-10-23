using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace ZenPlatform.XmlSerializer
{
    internal static class SerializerHelper
    {
        public static InstanceType GetInstanceType(object value)
        {
            return GetInstanceType(value.GetType());
        }

        public static InstanceType GetInstanceType(Type type)
        {
            switch (true)
            {
                case bool _ when type == typeof(bool):
                case bool _ when type == typeof(byte):
                case bool _ when type == typeof(byte[]):
                case bool _ when type == typeof(DateTime):
                case bool _ when type == typeof(DateTimeOffset):
                case bool _ when type == typeof(decimal):
                case bool _ when type == typeof(double):
                case bool _ when type == typeof(short):
                case bool _ when type == typeof(int):
                case bool _ when type == typeof(long):
                case bool _ when type == typeof(sbyte):
                case bool _ when type == typeof(float):
                case bool _ when type == typeof(string):
                case bool _ when type == typeof(TimeSpan):
                case bool _ when type == typeof(short):
                case bool _ when type == typeof(uint):
                case bool _ when type == typeof(ulong):
                    return InstanceType.Primitive;
                    break;
                case bool _ when type.GetInterface(nameof(IEnumerable)) != null:
                    return InstanceType.Collection;
                    break;
                default:
                    return InstanceType.Object;
                    break;
            }
        }

        public static bool InstanceTypeIsCollection(MemberInfo memberInfo)
        {
            return (memberInfo is PropertyInfo pi && SerializerHelper.GetInstanceType(pi.PropertyType) ==
                    InstanceType.Collection)
                   || (memberInfo is FieldInfo fi && SerializerHelper.GetInstanceType(fi.FieldType) ==
                       InstanceType.Collection);
        }

        public static Type GetUnderlaingType(this MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo pi)
                return pi.PropertyType;
            if (memberInfo is FieldInfo fi)
                return fi.FieldType;

            return null;
        }
    }

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
            _instanceType = SerializerHelper.GetInstanceType(instance);
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
            var valueType = SerializerHelper.GetInstanceType(value);
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


        public static string GetNameWithoutGenericArity(Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }


    public class XmlnsDefinitionAttribute : Attribute
    {
        public string Namespace { get; set; }
    }
}