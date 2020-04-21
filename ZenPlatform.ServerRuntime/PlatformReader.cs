using System;
using System.Collections.Generic;
using System.Data.Common;
using BufferedDataReaderDotNet;
using ZenPlatform.Core;
using ZenPlatform.Core.Querying.Model;
using ZenPlatform.SharedRuntime;

namespace ZenPlatform.ServerRuntime
{
    public abstract class PlatformReader
    {
        public virtual object this[string value]
        {
            get { return null; }
        }

        public virtual bool Read()
        {
            return false;
        }
    }

    public class ApplicationCachedPlatformReader : PlatformReader
    {
        private readonly DbDataReader _reader;
        private readonly QItem _logicalTree;
        private readonly PlatformContext _context;
        private BufferedData _buffered;
        private BufferedDataReader _bufferedReader;

        public ApplicationCachedPlatformReader(DbDataReader reader, QItem logicalTree, PlatformContext context)
        {
            _reader = reader;
            _logicalTree = logicalTree;
            _context = context;
            _buffered = _reader.GetBufferedData(BufferedDataOptions.Default);
            _bufferedReader = _buffered.GetDataReader();
        }

        public override object this[string value] => base[value];

        /*
         Link + Type
         
         Link = Type + Guid + Presentation
         
         
         ObjectFromDataFactory.Register(typeNumber, object args => new StoreLink((Guid)args[0]))
         GetFactory(int type).Action(reader[a], reader[b], reader[c])
        */


        public override bool Read()
        {
            return _bufferedReader.Read();
        }
    }
}