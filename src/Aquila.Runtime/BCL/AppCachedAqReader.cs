using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Querying.Model;
using BufferedDataReaderDotNet;

namespace Aquila.Data
{
    public class AppCachedAqReader : AqReader
    {
        private readonly DbDataReader _reader;
        private readonly QQuery _logicalQuery;
        private readonly AqContext _context;
        private BufferedData _buffered;
        private BufferedDataReader _bufferedReader;

        public AppCachedAqReader(DbDataReader reader, QQuery logicalQuery, AqContext context)
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
            var fields = _logicalQuery.Select.Fields.Elements;

            var columnIndex = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                var currentField = fields[i];

                if (currentField.GetName() == colName)
                {
                    return _bufferedReader.GetValue(i);
                }
            }

            throw new Exception($"Column {colName} not found");
        }

        private void Analyze()
        {
            var results = _logicalQuery.Select.Fields;

            foreach (var fieldResult in results)
            {
                var computedTypes = fieldResult.GetExpressionType().ToList();
            }
        }


        public override bool read()
        {
            return _bufferedReader.Read();
        }
    }
}