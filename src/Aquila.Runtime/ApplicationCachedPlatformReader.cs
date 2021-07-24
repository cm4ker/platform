using System;
using System.Data.Common;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Querying.Model;
using BufferedDataReaderDotNet;

namespace Aquila.Data
{
    public class ApplicationCachedPlatformReader : PlatformReader
    {
        private readonly DbDataReader _reader;
        private readonly QQuery _logicalQuery;
        private readonly PlatformContext _context;
        private BufferedData _buffered;
        private BufferedDataReader _bufferedReader;

        public ApplicationCachedPlatformReader(DbDataReader reader, QQuery logicalQuery, PlatformContext context)
        {
            _reader = reader;
            _logicalQuery = logicalQuery;
            _context = context;
            _buffered = _reader.GetBufferedData(BufferedDataOptions.Default);
            _bufferedReader = _buffered.GetDataReader();


            _reader.Dispose();
            Analyze();
        }

        public override object this[string value] => GetValue(value);


        public object GetValue(string colName)
        {
            throw new NotImplementedException();
        }

        private void Analyze()
        {
            var results = _logicalQuery.Select.Fields;

            foreach (var fieldResult in results)
            {
                var computedTypes = fieldResult.GetExpressionType().ToList();
            }
        }

        public override bool Read()
        {
            return _bufferedReader.Read();
        }
    }
}