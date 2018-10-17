using System;
using System.Data;
using System.IO;
using System.Xml;

namespace ZenPlatform.XmlSerializer
{
    public class XSConfiguration
    {
        public IXmlSerializer Build()
        {
            return null;
        }

        public void Handler(Type type, Action<object> handler)
        {
        }

        public void Handler<T>(Action<T> handler)
        {
            Handler(typeof(T), o => handler((T) o));
        }
    }

    public class XmlSerializer : IXmlSerializer
    {
        public void Serialize(object instance, TextWriter textWriter)
        {
            var xw = XmlWriter.Create(textWriter);
            //TODO: Добавить параметры для XmlWriter
            Serialize(instance, xw);
        }

        public void Serialize(object instance, XmlWriter textWriter)
        {
            var type = instance.GetType();
            foreach (var prop in type.GetProperties())
            {
                
            }
        }
    }


    public interface IXmlSerializer
    {
        void Serialize(object instance, TextWriter textWriter);
        void Serialize(object instance, XmlWriter textWriter);
    }
}