using System;
using System.Data.Common;
using dnlib.DotNet;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Core;
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
        private QItem _logicalTree;

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

            return new ApplicationCachedPlatformReader(_command.ExecuteReader(), _logicalTree, _context);
        }

        public void SetParameter(string paramName, object value)
        {
        }

        public void Execute()
        {
        }
    }
}