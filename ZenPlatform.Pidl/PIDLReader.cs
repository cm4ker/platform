using System;
using System.IO;
using System.Xml;
using Portable.Xaml;

namespace ZenPlatform.Pidl
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