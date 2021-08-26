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
using Aquila.ServerRuntime;

namespace Aquila.Core
{
    public class AqQuery
    {
        private string _text;
        private string _compiled;
        private bool _needRecompile;
        private PlatformContext _context;
        private DbCommand _command;
        private QQueryList _logicalTree;

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public AqQuery()
        {
            _context = PlatformContext.GetContext();
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
                var result = QueryCompilerHelper.Compile(_context.DataRuntimeContext, _text);

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

            return new AppCachedAqReader(_command.ExecuteReader(), _logicalTree.Last(), _context);
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
                                p.Value = GetTypeId(pr.Real);
                            }
                            else
                            {
                                p.Value = GetValue(pr.Real, dbSchema.Type);
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
                    //Parameter not joined (not provide?)
                }
            }
        }

        private int GetTypeId(object value)
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

        private object GetValue(object value, SMType type)
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
    }


    /*
    
    var a = 10;
     
     //Where B is string/int/ref type for example
     var q = query();
     
     q.Text = "FROM A SELECT B WHERE Id < @param1";
     q.SetParameter("param1", a);
     var col = q.Execute();
     
     iter(col -> elem)
     {
        var counter = 0;

        elem.B match  <-- Here we can access to the B element
        {
            string x => show_message(x);
            int i =>  counter  + i;
            Invoice inv => some_func(inv);
            Store s => some_func2(s);
            Nomenclature n => some_func3(n);
            _ => show_message("we can't do anything")
        } 

        match(elem.B)  <-- Here we can access to the B element
        {
            "Валенки" x => valenki(x);
            "Сапоги" y => sapogi(y);
            0 => nothing
        } 
     }
    
    
          
    */


    /*
     
     var a = 10;
     
     //Where B is string/int/ref type for example
     var rd = q"FROM A SELECT B WHERE Id < {a}";
     rd.Execute();
     
     iter(rd -> elem)
     {
        var counter = 0;

        elem.B match  <-- Here we can access to the B element
        {
            string x => show_message(x);
            int i =>  counter  + i;
            Invoice inv => some_func(inv);
            Store s => some_func2(s);
            Nomenclature n => some_func3(n);
            _ => show_message("we can't do anything")
        } 

        match(elem.B)  <-- Here we can access to the B element
        {
            "Валенки" x => valenki(x);
            "Сапоги" y => sapogi(y);
            0 => nothing
        } 
     }

     

     Invoice|int|string a = "test"
     
     a = a + 1 <---- error can't execute arithmetical ops on the union type only transfer data
     
     a = a match
            | int x => x
            | Invoice => 
                        {
                            some expression
                            last expression must return value
                        }
            | _ => 0
            
            + 1; 
     
     string|int b = t;
     Invoice|int b = 0;
     
     a = b;
     
     if(a == 0) 
        return true;
     else
        return false;     
     
     */
}