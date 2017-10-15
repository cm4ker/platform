using System.Collections.Generic;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class STable
    {
        public STable()
        {
            Fields = new List<SField>();
        }

        public string Name { get; set; }
        public List<SField> Fields { get; set; }
    }
}