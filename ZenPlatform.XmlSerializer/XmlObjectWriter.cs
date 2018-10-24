using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using ZenPlatform.XmlSerializer;

namespace ZenPlatform.XmlSerializer
{
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
                    var isInsideTheObject = _stateStack.Any() && _stateStack.Peek().IsObject;
                    var isCollection = _stateStack.Any() && _stateStack.Peek().IsCollection;

                    if (isInsideTheObject)
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

                            SetValue(prop, state.Instance, value);

                            _reader.ReadEndElement();
                        }
                        else
                        {
                            if (SerializerHelper.InstanceTypeIsCollection(prop))
                            {
                                object collectionInstance = GetValue(prop, state.Instance);

                                if (collectionInstance == null)
                                {
                                    collectionInstance = Activator.CreateInstance(prop.GetUnderlaingType());
                                    SetValue(prop, state.Instance, collectionInstance);
                                }

                                _stateStack.Push(new State()
                                {
                                    IsObject = false, IsCollection = true, Instance = collectionInstance,
                                    Namespaces = state.Namespaces
                                });
                            }
                            else
                                _stateStack.Push(new State()
                                {
                                    IsObject = false, IsCollection = false, Instance = state.Instance,
                                    MemberInfo = prop, Namespaces = state.Namespaces
                                });

                            if (_conf.TypesWithoutRoot.Contains(prop.GetUnderlaingType()))
                            {
                                object instance = GetValue(prop, state.Instance);

                                if (instance == null)
                                {
                                    instance = Activator.CreateInstance(prop.GetUnderlaingType());
                                }

                                if (!_reader.IsEmptyElement)
                                    _stateStack.Push(new State()
                                    {
                                        IsObject = true, IsCollection = false, Instance = instance, MemberInfo = prop,
                                        Namespaces = state.Namespaces
                                    });
                                else
                                    _stateStack.Pop();
                            }
                        }
                    }
                    else
                    {
                        var namespaces = new Dictionary<string, string>();
                        var typeName = _reader.Name;

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

                            _reader.Read();
                        }

                        if (_stateStack.Any() && _stateStack.Peek().Namespaces != null)
                        {
                            foreach (var parentNs in _stateStack.Peek().Namespaces)
                            {
                                namespaces.Add(parentNs.Key, parentNs.Value);
                            }
                        }

                        var type = GetTypeUsingNamespace(typeName, namespaces);

                        if (type == null) throw new Exception($"we can't determinate the type {typeName}");

                        var instType = SerializerHelper.GetInstanceType(type);

                        object instance;
                        if (isCollection && instType == InstanceType.Primitive)
                        {
                            // читаем значение
                            _reader.Read();

                            var value = (new TypeConverter()).ConvertTo(_reader.Value, type);
                            instance = value;

                            //читаем конец элемента
                            _reader.Read();
                        }
                        else
                        {
                            instance = Activator.CreateInstance(type);
                        }


                        if (_stateStack.Any())
                        {
                            var parent = _stateStack.Peek();

                            if (isCollection)
                            {
                                ((IList) parent.Instance).Add(instance);
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
                                IsCollection = instType == InstanceType.Collection, Instance = instance,
                                IsObject = true, Namespaces = namespaces
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

        private object GetValue(MemberInfo member, object instance)
        {
            if (member is PropertyInfo pi)
                return pi.GetValue(instance);
            if (member is FieldInfo fi)
                return fi.GetValue(instance);

            throw new Exception("Wrong member");
        }

        private void ReadWhile(XmlNodeType nodeType)
        {
            while (_reader.NodeType != nodeType && _reader.Read())
            {
            }
        }


        private Type GetTypeUsingNamespace(string typeNameWithPrefix, Dictionary<string, string> namespaces)
        {
            string typeName;
            string preffix = string.Empty;
            var args = typeNameWithPrefix.Split(':');
            if (args.Length > 1)
            {
                typeName = args[1];
                preffix = args[0];
            }
            else
            {
                typeName = args[0];
            }

            var standardAssemblies = new List<Assembly>();
            var connectedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            /*
             * xmlns="http://some-space.com/test"
             * xmlns="clr-namespace: Assembly1.Test"
             */
            foreach (var ns in namespaces)
            {
                if (string.IsNullOrEmpty(preffix) && !string.IsNullOrEmpty(ns.Key) || preffix != ns.Key) continue;
                foreach (var asm in connectedAssemblies)
                {
                    var attrs = asm.GetCustomAttributes(typeof(XmlnsDefinitionAttribute));
                    foreach (XmlnsDefinitionAttribute attr in attrs)
                    {
                        if (attr.Namespace == ns.Value)
                        {
                            if (_conf.CustomRoots.TryGetValue(typeName, out var replaceType))
                            {
                                return asm.GetTypes().FirstOrDefault(x => x == replaceType);
                            }

                            return asm.GetTypes().FirstOrDefault(x => x.Name == typeName);
                        }
                    }
                }
            }

            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == typeName);
        }
    }
}