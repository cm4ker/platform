using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data
{
    public class PConfig
    {
        private List<PObjectType> _objects;

        public PConfig()
        {
            _objects = new List<PObjectType>();
        }

        public List<PObjectType> Objects => _objects;
    }
}