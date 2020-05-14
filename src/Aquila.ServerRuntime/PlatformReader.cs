using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Avalonia;
using BufferedDataReaderDotNet;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Core;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.SharedRuntime;

namespace Aquila.ServerRuntime
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
        private readonly QQuery _logicalQuery;
        private readonly PlatformContext _context;
        private BufferedData _buffered;
        private BufferedDataReader _bufferedReader;
        private ITypeManager _tm;

        public ApplicationCachedPlatformReader(DbDataReader reader, QQuery logicalQuery, PlatformContext context)
        {
            _reader = reader;
            _logicalQuery = logicalQuery;
            _context = context;
            _tm = context.TypeManager;
            _buffered = _reader.GetBufferedData(BufferedDataOptions.Default);
            _bufferedReader = _buffered.GetDataReader();


            _reader.Dispose();
            Analyze();
        }

        public override object this[string value] => GetValue(value);


        public object GetValue(string colName)
        {
            var cols = _map[colName];

            string colNameValue;

            if (cols.Count > 1)
            {
                var colType = cols.FirstOrDefault(x => x.SchemaType == ColumnSchemaType.Type);

                if (colType == null)
                    throw new Exception(
                        "column type is not defined for multi column value. This never must happen. Crush!");

                var typeId = (int) _bufferedReader[colType.FullName];

                var valueColumn = cols.FirstOrDefault(x =>
                    x.PlatformIpType.GetSettings().SystemId == typeId && x.SchemaType != ColumnSchemaType.Type);

                if (valueColumn == null)
                    throw new Exception("Value column is not defined");

                colNameValue = valueColumn.FullName;

                if (valueColumn.PlatformIpType.IsLink)
                {
                    var linkId = (Guid) _bufferedReader[colNameValue];

                    //TODO: Replace presentation Unknown
                    _context.LinkFactory.Create(typeId, linkId, "Unknown");
                }
            }
            else
            {
                colNameValue = cols.First().FullName;
            }

            return _bufferedReader[colNameValue];
        }

        private Dictionary<string, List<ColumnSchemaDefinition>> _map =
            new Dictionary<string, List<ColumnSchemaDefinition>>();

        private void Analyze()
        {
            var results = _logicalQuery.Select.Fields;

            foreach (var fieldResult in results)
            {
                var computedTypes = fieldResult.GetExpressionType().ToList();

                var schemas = _tm.GetPropertySchemas(fieldResult.GetDbName(), computedTypes).ToList();

                var fields = new List<ColumnSchemaDefinition>();
                fields.AddRange(schemas);

                _map[fieldResult.GetName()] = fields;
            }
        }

        public override bool Read()
        {
            return _bufferedReader.Read();
        }
    }
}