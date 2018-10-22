using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    /// <summary>
    /// Внутренний класс для обработки xml-контента в объект 
    /// </summary>
    internal class XmlObjectWriter
    {
        private struct State
        {
            public bool IsCollection { get; set; }
            public bool IsObject { get; set; }
            public object Instance { get; set; }
            public MemberInfo MemberInfo { get; set; }
            public Dictionary<string, string> Namespaces { get; set; }
        }

        private readonly Type _targetType;
        private readonly XmlReader _reader;
        private readonly SerializerConfiguration _conf;
        private object _result;
        private Stack<State> _stateStack;

        public XmlObjectWriter(XmlReader reader, SerializerConfiguration configuration)
        {
            _reader = reader;
            _conf = configuration;
            _stateStack = new Stack<State>();
        }

        public object Handle()
        {
            object _result = null;

            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    var isObject = !_stateStack.Any() || (_stateStack.Any() && !_stateStack.Peek().IsObject);
                    var isCollection = _stateStack.Any() && _stateStack.Peek().IsCollection;
                    if (!isObject)
                    {
                        var state = _stateStack.Peek();
                        var type = state.Instance.GetType();


                        MemberInfo prop = type.GetProperty(_reader.Name);
                        if (prop == null)
                            prop = type.GetField(_reader.Name);
                        if (prop == null) throw new Exception("We can't write this");

                        var instType = SerializerHelper.GetInstanceType(type);
                        if (instType == InstanceType.Primitive)
                        {
                            var value = (new TypeConverter()).ConvertTo(_reader.Value, type);

                            if (prop is PropertyInfo pi)
                                pi.SetValue(state.Instance, value);
                            if (prop is FieldInfo fi)
                                fi.SetValue(state.Instance, value);

                            _reader.ReadEndElement();
                        }
                        else
                        {
                            if (SerializerHelper.InstanceTypeIsCollection(prop))
                            {
                                var collectionInstance = Activator.CreateInstance(prop.GetUnderlaingType());

                                SetValue(prop, state.Instance, collectionInstance);

                                _stateStack.Push(new State()
                                    {IsObject = false, IsCollection = true, Instance = collectionInstance});
                            }
                            else
                                _stateStack.Push(new State()
                                {
                                    IsObject = false, IsCollection = false, Instance = state.Instance, MemberInfo = prop
                                });
                        }
                    }
                    else
                    {
                        var namespaces = new Dictionary<string, string>();

                        //Проверяем наличие атрибутов
                        if (_reader.HasAttributes)
                        {
                            _reader.MoveToFirstAttribute();
                            do
                            {
                                //сканируем на наличие атрибутов-пространств имён
                                if (_reader.Name.Contains("xmlns"))
                                {
                                    var name = (_reader.Name.Contains(":")
                                        ? _reader.Name.Substring(6)
                                        : _reader.Name.Substring(5));

                                    namespaces.Add(name, _reader.Value);
                                }
                            } while (_reader.MoveToNextAttribute());
                        }

                        if (_stateStack.Any())
                        {
                            foreach (var parentNs in _stateStack.Peek().Namespaces)
                            {
                                namespaces.Add(parentNs.Key, parentNs.Value);
                            }
                        }

                        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                            .Where(x => x.Name == _reader.Name).ToArray();

                        if (types.Count() != 1) throw new Exception("we can't determinate the type ");

                        var instType = SerializerHelper.GetInstanceType(types[0]);

                        object instance;
                        if (isCollection && instType == InstanceType.Primitive)
                        {
                            // читаем значение
                            _reader.Read();

                            var value = (new TypeConverter()).ConvertTo(_reader.Value, types[0]);
                            instance = value;

                            //читаем конец элемента
                            _reader.Read();
                        }
                        else
                        {
                            instance = Activator.CreateInstance(types[0]);
                        }


                        if (_stateStack.Any())
                        {
                            var parent = _stateStack.Peek();

                            if (isCollection)
                            {
                                ((IList) parent.Instance).Add(instance);
                                continue;
                            }
                            else
                            {
                                if (parent.MemberInfo is PropertyInfo pi)
                                    pi.SetValue(parent.Instance, instance);
                                if (parent.MemberInfo is FieldInfo fi)
                                    fi.SetValue(parent.Instance, instance);
                            }
                        }

                        if (!_reader.IsEmptyElement)
                        {
                            _stateStack.Push(new State
                            {
                                IsCollection = instType == InstanceType.Collection, Instance = instance, IsObject = true
                            });
                        }
                    }
                }
                else if (_reader.NodeType == XmlNodeType.EndElement)
                {
                    var state = _stateStack.Pop();
                    if (_stateStack.Count == 0) _result = state.Instance;
                }
                else if (_reader.NodeType == XmlNodeType.Attribute)
                {
                    var state = _stateStack.Peek();
                    if (_reader.Name.StartsWith("xmlns"))
                    {
                        state.Namespaces.Add(_reader.Name.Replace("xmlns:", ""), _reader.Value);
                    }
                }
            }

            return _result;
        }

        private void SetValue(MemberInfo member, object instance, object value)
        {
            if (member is PropertyInfo pi)
                pi.SetValue(instance, value);
            if (member is FieldInfo fi)
                fi.SetValue(instance, value);
        }

        private void ReadWhile(XmlNodeType nodeType)
        {
            while (_reader.NodeType != nodeType && _reader.Read())
            {
            }
        }


        private Type GetTypeUsingNamespace(string typeName, Dictionary<string, string> namespaces)
        {
            // 
          
            AppDomain.CurrentDomain.GetAssemblies().Where(x)
        }
    }
}