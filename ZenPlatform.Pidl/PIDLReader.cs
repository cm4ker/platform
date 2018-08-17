using System;
using System.IO;
using System.Xml;

namespace ZenPlatform.Pidl
{
    public class PIDLReader : XmlReader
    {
        private readonly TextReader _reader;

        public PIDLReader(Stream stream) : base()
        {
            _reader = new StreamReader(stream);
        }

        public PIDLReader(TextReader reader)
        {
            _reader = reader;
        }

        public override string GetAttribute(int i)
        {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            throw new NotImplementedException();
        }

        public override string LookupNamespace(string prefix)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToElement()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToFirstAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNextAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }

        public override bool ReadAttributeValue()
        {
            throw new NotImplementedException();
        }

        public override void ResolveEntity()
        {
            throw new NotImplementedException();
        }

        public override int AttributeCount { get; }
        public override string BaseURI { get; }
        public override int Depth { get; }
        public override bool EOF { get; }
        public override bool IsEmptyElement { get; }
        public override string LocalName { get; }
        public override string NamespaceURI { get; }
        public override XmlNameTable NameTable { get; }
        public override XmlNodeType NodeType { get; }
        public override string Prefix { get; }
        public override ReadState ReadState { get; }
        public override string Value { get; }
    }
}