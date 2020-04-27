using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using dnlib.DotNet;
using Microsoft.Win32.SafeHandles;
using MoreLinq;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Core;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Querying;
using ZenPlatform.Core.Querying.Model;

namespace ZenPlatform.ServerRuntime
{
    /*
     Query plan:
     
     1) Get data and store it in tmp table(+unique key) on RDBMS 
     2) 
     
     */


    public class PlatformQuery
    {
        private ITypeManager _tm;
        private string _text;
        private string _compiled;
        private bool _needRecompile;
        private PlatformContext _context;
        private DbCommand _command;
        private QQueryList _logicalTree;

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public PlatformQuery()
        {
            _context = ContextHelper.GetContext();

            if (!_context.IsTypeManagerAvailable)
                throw new Exception("Type manager not available. Crush!");
            _tm = _context.TypeManager;

            _command = _context.DataContext.CreateCommand();
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _needRecompile = true;
            }
        }

        public PlatformReader ExecuteReader()
        {
            if (_needRecompile)
            {
                var result = QueryCompilerHelper.Compile(_tm, _text);

                /*
                 
                 SELECT 
                    A1, A2, A3, A4, A5 ....
                    ^   ^   ^   ^   ^
                   MappedTo O1  MappedTo O2
                 FROM
                    T1 ....
                 
                 1) Link (StoreLink, InvoiceLink ... etc)
                 2) Literal (Guid, String, int, Double ... etc)
                 
                 => {
                 
                    Reader.Invoice => Manager.Invoice.Get((Guid)Reader["Fld_255"]);
                    
                    Reader.Link => 
                    
                 
                 }
                                
                 */

                _compiled = result.sql;
                _logicalTree = result.logicalTree;
            }


            _command.CommandText = _compiled;
            _command.Parameters.Clear();

            FillParametersCore();

            return new ApplicationCachedPlatformReader(_command.ExecuteReader(), _logicalTree.Last(), _context);
        }


        private void FillParametersCore()
        {
            var logicParams = _logicalTree.Find<QParameter>().ToList();
            var result = (from p in _parameters
                join l in logicParams
                    on p.Key equals l.Name
                    into t
                from d in t.DefaultIfEmpty()
                select new {Name = p.Key, Logic = d, Real = p.Value});


            foreach (var pr in result)
            {
                if (pr.Logic != null)
                {
                    var expType = pr.Logic.GetExpressionType().ToList();

                    if (expType.Count() > 1)
                    {
                        var schema = _tm.GetPropertySchemas(pr.Logic.GetDbName(), expType);

                        foreach (var sd in schema)
                        {
                            var p = _command.CreateParameter();
                            p.ParameterName = sd.FullName;

                            if (sd.SchemaType == ColumnSchemaType.Type)
                            {
                                p.Value = GetTypeId(pr.Real);
                            }
                            else
                            {
                                p.Value = GetValue(pr.Real, sd.PlatformIpType);
                            }
                            
                            _command.Parameters.Add(p);
                        }
                    }
                    else
                    {
                        var p = _command.CreateParameter();
                        p.ParameterName = pr.Name;
                        p.Value = GetValue(pr.Real, expType.First());

                        _command.Parameters.Add(p);
                    }
                }
                else
                {
                    //Debug the parameter not using
                }
            }
        }


        private int GetTypeId(object value)
        {
            return value switch
            {
                int a => (int) _tm.Int.GetSettings().SystemId,
                DateTime a => (int) _tm.DateTime.GetSettings().SystemId,
                string a => (int) _tm.String.GetSettings().SystemId,
                bool a => (int) _tm.Boolean.GetSettings().SystemId,
                Guid a => (int) _tm.Guid.GetSettings().SystemId,
                byte[] a => (int) _tm.Binary.GetSettings().SystemId,
                ILink a => a.TypeId,
            };
        }

        private IPType ToPlatformPrimitiveType(object value)
        {
            return value switch
            {
                int a => _tm.Int,
                DateTime a => _tm.DateTime,
                string a => _tm.String,
                bool a => _tm.Boolean,
                Guid a => _tm.Guid,
                byte[] a => _tm.Binary,
                _ => null,
            };
        }

        private object GetDefaultClr(IPType type)
        {
            return type.PrimitiveKind switch
            {
                PrimitiveKind.String => _context.DataContext.Types.String.DefaultValue,
                PrimitiveKind.Int => _context.DataContext.Types.Int.DefaultValue,
                PrimitiveKind.Binary => _context.DataContext.Types.Binary.DefaultValue,
                PrimitiveKind.Boolean => _context.DataContext.Types.Boolean.DefaultValue,
                PrimitiveKind.DateTime => _context.DataContext.Types.DateTime.DefaultValue,
                PrimitiveKind.Guid => _context.DataContext.Types.Guid.DefaultValue,
                PrimitiveKind.Numeric => _context.DataContext.Types.Numeric.DefaultValue,
                _ => DBNull.Value,
            };
        }

        private object GetValue(object value, IPType type)
        {
            if (Equals(type, ToPlatformPrimitiveType(value)))
            {
                return value;
            }

            if (type.IsLink && value is ILink l)
            {
                return l.Id;
            }
            else if (type.IsLink)
            {
                return Guid.Empty;
            }

            return GetDefaultClr(type);
        }

        public void SetParameter(string paramName, object value)
        {
            _parameters.Add(paramName, value);
        }

        public void Execute()
        {
        }
    }
}