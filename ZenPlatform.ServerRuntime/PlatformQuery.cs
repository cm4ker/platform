using System;
using System.Data.Common;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Core;
using ZenPlatform.Core.Querying;

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
                _compiled = ServerCompilerHelper.Compile(_tm, _text);

            _command.CommandText = _compiled;

            return new PlatformReader();
        }

        public void Execute()
        {
        }
    }
}