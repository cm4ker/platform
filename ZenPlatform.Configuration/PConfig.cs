using System.Collections.Generic;

namespace SqlPlusDbSync.Configuration
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