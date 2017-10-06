using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlPlusDbSync.Live.Models
{
    public class ActiveSessionModel
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public object Type { get; set; }
        public int Size { get; set; }
        public string Info { get; set; }

        public DateTime EventTime { get; set; }
    }
}
