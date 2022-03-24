using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using Aquila.Core;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Metadata;
using BufferedDataReaderDotNet;

namespace Aquila.Data
{
    public class AqCachedReader : AqReader
    {
        private readonly DbDataReader _reader;
        private readonly QSelectQuery _logicalQuery;
        private readonly AqContext _context;
        private BufferedData _buffered;
        private BufferedDataReader _bufferedReader;

        private Dictionary<string, IColumnReader> _readers;
        private Dictionary<int, IColumnReader> _readersIndex;

        public AqCachedReader(DbDataReader reader, QSelectQuery logicalQuery, AqContext context)
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

        public override object this[int index] => GetValue(index);


        public object GetValue(int colIndex)
        {
            return _readersIndex[colIndex].ReadValue(_bufferedReader);
        }

        public object GetValue(string colName)
        {
            return _readers[colName].ReadValue(_bufferedReader);
        }

        /*
                                
        Name => getStrategy
                                     
        Simple strategy
           reftype return Activator.CreateInstance(PTypes[typeId], _context, _bufferedReader[refCol], "Presentation", typeId) 
           primitivetype return (cast type from instance)_bufferedReader[valueCol]
        
        Complex strategy
        
        Dict[int, ClrType]
        var typeId = _bufferedReader[typeCol];
        if(typeId > 100)
           return Activator.CreateInstance(PTypes[typeId], _context, _bufferedReader[refCol], "Presentation", typeId)
        else
           return 
           switch(typeId)
           {
               1 => _bufferedReader[intCol];
               2 => _bufferedReader[stringCol];
               3 => _bufferedReader[dateTimeCol];                                    
           }
           */

        /*
                 field - set of types
                 
                 string, int, double, Link, Link
                                        \    /
                             need constructor for this types
                 1) Get platform types
                 2) Get classes
                 3) Create types
                 
                 */

        private void Analyze()
        {
            var results = _logicalQuery.Select.Fields.Elements;

            _readers = new Dictionary<string, IColumnReader>();
            _readersIndex = new Dictionary<int, IColumnReader>();
            var colIndex = 0; //first column

            foreach (var column in results)
            {
                IColumnReader reader;

                if (column.IsComplexExprType)
                {
                    reader = new ComplexColumnReader(_context, column);
                }
                else
                {
                    reader = new SimpleColumnReader(_context, column);
                }

                reader.Init(ref colIndex);

                _readers[column.GetName()] = reader;
                _readersIndex[_readersIndex.Count] = reader;
            }
        }


        public override bool read()
        {
            return _bufferedReader.Read();
        }


        private class ComplexColumnReader : IColumnReader
        {
            private readonly AqContext _context;
            private readonly QField _column;

            private Dictionary<int, Type> _runtimeTypes = new Dictionary<int, Type>();

            public ComplexColumnReader(AqContext context, QField column)
            {
                _context = context;
                _column = column;
            }

            public void Init(ref int colIndex)
            {
                var types = _column.GetExpressionType().GetOrderedFlattenTypes();
                foreach (var type in types)
                {
                    var colName = _column.GetDbName() + type.postfix;

                    if (type.isType)
                    {
                        _typeCol = colIndex++;
                        continue;
                    }

                    switch (type.type.Kind)
                    {
                        case SMTypeKind.Unknown:
                            throw new Exception("Unknown type");
                            break;
                        case SMTypeKind.String:
                            _stringCol = colIndex++;
                            break;
                        case SMTypeKind.Int:
                            _intCol = colIndex++;
                            break;
                        case SMTypeKind.Long:
                            break;
                        case SMTypeKind.Bool:
                            _boolCol = colIndex++;
                            break;
                        case SMTypeKind.Double:
                            break;
                        case SMTypeKind.Decimal:
                            break;
                        case SMTypeKind.DateTime:
                            _dateTimeCol = colIndex++;
                            break;
                        case SMTypeKind.Numeric:
                            break;
                        case SMTypeKind.Binary:
                            break;
                        case SMTypeKind.Guid:
                            break;
                        case SMTypeKind.Reference:
                            if (_refCol == -1)
                                _refCol = colIndex++;

                            var runtimeType =
                                _context.Instance.BLAssembly.GetType(type.type.GetSemantic().ReferenceName);
                            var descriptor =
                                _context.DataRuntimeContext.Descriptors.GetEntityDescriptor(type.type.GetSemantic()
                                    .FullName);
                            _runtimeTypes.Add(descriptor.DatabaseId, runtimeType);

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private int _typeCol = -1;
            private int _refCol = -1;
            private int _intCol = -1;
            private int _boolCol = -1;
            private int _stringCol = -1;
            private int _dateTimeCol = -1;

            public string ColumnName => _column.GetName();

            public object ReadValue(BufferedDataReader dataReader)
            {
                var typeId = (int)dataReader[_typeCol];

                if (typeId >= (int)SMTypeKind.Reference)
                {
                    var runtimeType = _runtimeTypes[typeId];
                    return Activator.CreateInstance(runtimeType, _context, (Guid)dataReader[_refCol]);
                }
                else
                {
                    var prType = (SMTypeKind)typeId;
                    return prType switch
                    {
                        SMTypeKind.String => dataReader[_stringCol],
                        SMTypeKind.Int => dataReader[_intCol],
                        SMTypeKind.Long => dataReader[_stringCol],
                        SMTypeKind.Bool => dataReader[_boolCol],
                        SMTypeKind.Double => dataReader[_stringCol],
                        SMTypeKind.Decimal => dataReader[_stringCol],
                        SMTypeKind.DateTime => dataReader[_dateTimeCol],
                        SMTypeKind.Numeric => dataReader[_stringCol],
                        SMTypeKind.Binary => dataReader[_stringCol],
                        SMTypeKind.Guid => dataReader[_stringCol],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }
        }

        private class SimpleColumnReader : IColumnReader
        {
            private readonly AqContext _context;
            private readonly QField _column;

            public SimpleColumnReader(AqContext context, QField column)
            {
                _context = context;
                _column = column;
            }

            //private string _colName;
            private int _colIndex;

            private int _typeId;
            private bool _isReference;
            private Type _runtimeType;

            public void Init(ref int colIndex)
            {
                _colIndex = colIndex++;
                //_colName = _column.GetDbName();
                var colType = _column.GetExpressionType().FirstOrDefault() ??
                              throw new Exception("Result type is empty");
                if (colType.IsReference)
                {
                    _runtimeType = _context.Instance.BLAssembly.GetType(colType.GetSemantic().ReferenceName);
                    _isReference = true;
                }
                else
                {
                }
            }

            public string ColumnName => _column.GetName();

            public object ReadValue(BufferedDataReader dataReader)
            {
                if (_isReference)
                    return Activator.CreateInstance(_runtimeType, _context, (Guid)dataReader[_colIndex]);

                return dataReader[_colIndex];
            }
        }

        private interface IColumnReader
        {
            string ColumnName { get; }
            object ReadValue(BufferedDataReader dataReader);

            //Initialize column and move index for internal columns count
            void Init(ref int colIndex);
        }
    }
}