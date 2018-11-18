using System;

namespace ZenPlatform.Configuration
{
    public class XCSimpleCounter : IXCConfigurationUniqueCounter
    {
        private uint _maxId = 100;
        
        public void GetId(Guid confId, ref uint oldId)
        {
            if (oldId != 0)
                return;

            oldId = _maxId++;
        }
    }
}