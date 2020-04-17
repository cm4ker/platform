using System;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Core;

namespace ZenPlatform.ServerRuntime
{
    public class PlatformQuery
    {
        private ITypeManager _tm;

        public PlatformQuery()
        {
            if (!ContextHelper.GetContext().IsTypeManagerAvailable)
                throw new Exception("Type manager not available. Crush!");
            _tm = ContextHelper.GetContext().TypeManager;
        }

        public string Text { get; set; }

        public PlatformReader ExecuteReader()
        {
            return null;
        }

        public void Execute()
        {
        }
    }
}