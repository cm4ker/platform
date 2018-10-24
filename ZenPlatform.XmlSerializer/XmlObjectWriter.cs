using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ZenPlatform.XmlSerializer
{
    /// <summary>
    /// Внутренний класс для обработки xml-контента в объект 
    /// </summary>
    internal class XmlObjectWriter
    {
        private readonly Type _targetType;
        private readonly XmlReader _reader;
        private readonly SerializerConfiguration _conf;


        private Stack<State> _states;

        public XmlObjectWriter(XmlReader reader, SerializerConfiguration configuration)
        {
            _reader = reader;
            _conf = configuration;
        }

        public void Handle()
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element)
                {
                    var typeName = _reader.Name;
                    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                        .Where(x => x.Name == typeName);
                    var type = types.FirstOrDefault();

                    _states.Push(new State() {Type = type, Instance = Activator.CreateInstance(type)});
                }
                else (_reader.NodeType == XmlNodeType.Attribute)
            }
        }
    }


    internal struct State
    {
        public Type Type { get; set; }
        public object Instance { get; set; }
        public 
    }
}