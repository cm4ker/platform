using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Aquila.Core.Contracts;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;
using Aquila.Data;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.Runtime;

namespace Aquila.Core
{
    public class AqQuery : IDisposable
    {
        private string _text;
        private string _compiled;
        private bool _needRecompile;
        private AqContext _context;
        private DbCommand _command;
        private QQueryList _logicalTree;

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public AqQuery(AqContext context)
        {
            _context = context;
            _command = _context.DataContext.CreateCommand();
        }

        public string text
        {
            get => _text;
            set
            {
                _text = value;
                _needRecompile = true;
            }
        }

        public AqReader exec()
        {
            if (_needRecompile)
            {
                var result = QLangHelper.Compile(_context, _text);

                _compiled = result.sql;
                _logicalTree = result.logicalTree;
            }

            _command.CommandText = _compiled;
            _command.Parameters.Clear();

            FillParametersCore();

            //we think what last query in chain is select Query
            //TODO: throw platform exception if sQuery is null
            var sQuery = _logicalTree.Last() as QSelectQuery;
            return new AqCachedReader(_command.ExecuteReader(), sQuery, _context);
        }

        public T scalar<T>()
        {
            var r = exec();

            if (!r.read())
            {
                return default;
            }

            try
            {
                return (T)r[0];
            }
            catch
            {
                return default;
            }
        }

        private void FillParametersCore()
        {
            var logicParams = _logicalTree.Find<QParameter>().ToList();
            var result = (from p in _parameters
                join l in logicParams
                    on p.Key equals l.Name
                    into t
                from d in t.DefaultIfEmpty()
                select new { Name = p.Key, QElem = d, Real = p.Value });

            foreach (var pr in result)
            {
                if (pr.QElem != null)
                {
                    var expType = pr.QElem.GetExpressionType().ToList();

                    if (expType.Count() > 1)
                    {
                        var dbSchemas = DRContextHelper.GetPropertySchemas(pr.QElem.GetDbName(), expType);

                        foreach (var dbSchema in dbSchemas)
                        {
                            var p = _command.CreateParameter();
                            p.ParameterName = dbSchema.FullName;

                            if (dbSchema.SchemaType == ColumnSchemaType.Type)
                            {
                                p.Value = GetTypeIdFromValue(pr.Real);
                            }
                            else
                            {
                                p.Value = GetValueForParameter(pr.Real, dbSchema.Type);
                            }

                            _command.Parameters.Add(p);
                        }
                    }
                    else
                    {
                        var p = _command.CreateParameter();
                        p.ParameterName = pr.Name;
                        p.Value = GetValueForParameter(pr.Real, expType.First());
                        _command.Parameters.Add(p);
                    }
                }
                else
                {
                    //Parameter not joined (not provide?)
                }
            }
        }

        private int GetTypeIdFromValue(object value)
        {
            return value switch
            {
                int a => (int)SMTypeKind.Int,
                DateTime a => (int)SMTypeKind.DateTime,
                string a => (int)SMTypeKind.String,
                bool a => (int)SMTypeKind.Bool,
                Guid a => (int)SMTypeKind.Guid,
                byte[] a => (int)SMTypeKind.Binary,
                ILink a => a.TypeId,
            };
        }

        private object GetDefaultClr(SMType type)
        {
            return type.Kind switch
            {
                SMTypeKind.Unknown => _context.DataContext.Types.String.DefaultValue,
                SMTypeKind.String => _context.DataContext.Types.String.DefaultValue,
                SMTypeKind.Int => _context.DataContext.Types.Int.DefaultValue,
                SMTypeKind.Long => _context.DataContext.Types.Int.DefaultValue,
                SMTypeKind.Bool => _context.DataContext.Types.Boolean.DefaultValue,
                SMTypeKind.Double => _context.DataContext.Types.Numeric.DefaultValue,
                SMTypeKind.Decimal => _context.DataContext.Types.Numeric.DefaultValue,
                SMTypeKind.DateTime => _context.DataContext.Types.DateTime.DefaultValue,
                SMTypeKind.Numeric => _context.DataContext.Types.Numeric.DefaultValue,
                SMTypeKind.Binary => _context.DataContext.Types.Binary.DefaultValue,
                SMTypeKind.Guid => _context.DataContext.Types.Guid.DefaultValue,
                SMTypeKind.Reference => _context.DataContext.Types.Guid.DefaultValue,
                _ => DBNull.Value,
            };
        }

        private object GetValueForParameter(object value, SMType type)
        {
            if (type.IsPrimitive)
            {
                return value;
            }

            if (type.IsReference && value is ILink l)
            {
                return l.Id;
            }
            else if (type.IsReference)
            {
                return Guid.Empty;
            }

            return GetDefaultClr(type);
        }

        public void set_param(string paramName, object value)
        {
            _parameters.Add(paramName, value);
        }

        public void Dispose()
        {
            //Dispose command after using it (no memory consumption)
            _command?.Dispose();
        }
    }
}