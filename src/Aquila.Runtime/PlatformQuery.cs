using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Aquila.Core;
using Aquila.Core.Querying;
using Aquila.Core.Querying.Model;

namespace Aquila.Data
{
    public class PlatformQuery
    {
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
                // var result = QueryCompilerHelper.Compile(_tm, _text);
                //
                // /*
                //  
                //  SELECT 
                //     A1, A2, A3, A4, A5 ....
                //     ^   ^   ^   ^   ^
                //    MappedTo O1  MappedTo O2
                //  FROM
                //     T1 ....
                //  
                //  1) Link (StoreLink, InvoiceLink ... etc)
                //  2) Literal (Guid, String, int, Double ... etc)
                //  
                //  => {
                //  
                //     Reader.Invoice => Manager.Invoice.Get((Guid)Reader["Fld_255"]);
                //     
                //     Reader.Link => 
                //     
                //  
                //  }
                //                 
                //  */
                //
                // _compiled = result.sql;
                // _logicalTree = result.logicalTree;
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
                select new { Name = p.Key, Logic = d, Real = p.Value });


            foreach (var pr in result)
            {
                if (pr.Logic != null)
                {
                    var expType = pr.Logic.GetExpressionType().ToList();

                    if (expType.Count() > 1)
                    {
                    }
                    else
                    {
                    }
                }
                else
                {
                    //Debug the parameter not using
                }
            }
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