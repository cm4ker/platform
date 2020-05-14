using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;

namespace Aquila.XmlSerializer
{
    public class Serializer : IXmlSerializer
    {
        public void Serialize(object instance, TextWriter textWriter, SerializerConfiguration configuration = null)
        {
            using (var xw = XmlWriter.Create(textWriter))
                //TODO: Добавить параметры для XmlWriter
                Serialize(instance, xw, configuration);
        }

        public void Serialize(object instance, XmlWriter xmlWriter, SerializerConfiguration configuration = null)
        {
            var writer = new ObjectXmlWriter(instance, xmlWriter, configuration);
            writer.Handle();
        }

        public object Deserialize(string content, SerializerConfiguration configuration = null)
        {
            using (var tr = new StringReader(content))
                return Deserialize(tr, configuration);
        }

        public object Deserialize(TextReader reader, SerializerConfiguration configuration = null)
        {
            using (var xr = XmlReader.Create(reader))
                return Deserialize(xr, configuration);
        }

        public object Deserialize(XmlReader reader, SerializerConfiguration configuration = null)
        {
            var writer = new XmlObjectWriter(reader, configuration);
            return writer.Handle();
        }

        public object Deserialize(Stream stream, SerializerConfiguration configuration = null)
        {
            using (var test = XmlReader.Create(stream))
            {
                string output = "";
                while (test.Read())
                {
                    output += $"Type={test.NodeType} Name={test.Name} Value={test.Value} \n";
                }
            }

            stream.Position = 0;

            using (var xr = XmlReader.Create(stream))
                return Deserialize(xr, configuration);
        }

        public T Deserialize<T>(string content, SerializerConfiguration configuration = null)
        {
            return (T) Deserialize(content, configuration);
        }

        public T Deserialize<T>(TextReader reader, SerializerConfiguration configuration = null)
        {
            return (T) Deserialize(reader, configuration);
        }

        public T Deserialize<T>(XmlReader reader, SerializerConfiguration configuration = null)
        {
            return (T) Deserialize(reader, configuration);
        }

        public T Deserialize<T>(Stream stream, SerializerConfiguration configuration = null)
        {
            return (T) Deserialize(stream, configuration);
        }
    }

    public interface IXmlSerializer
    {
        void Serialize(object instance, TextWriter textWriter, SerializerConfiguration configuration = null);
        void Serialize(object instance, XmlWriter textWriter, SerializerConfiguration configuration = null);

        object Deserialize(string content, SerializerConfiguration configuration = null);
        object Deserialize(TextReader reader, SerializerConfiguration configuration = null);
        object Deserialize(XmlReader reader, SerializerConfiguration configuration = null);
        object Deserialize(Stream stream, SerializerConfiguration configuration = null);

        T Deserialize<T>(string content, SerializerConfiguration configuration = null);
        T Deserialize<T>(TextReader reader, SerializerConfiguration configuration = null);
        T Deserialize<T>(XmlReader reader, SerializerConfiguration configuration = null);
    }
}