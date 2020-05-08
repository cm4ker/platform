using System;
using System.IO;
using System.Xml;

namespace Aquila.Pidl
{
    public class PIDLReader
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

    }
}