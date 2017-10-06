using System.Collections.Generic;

namespace SqlPlusDbSync.Data.Model
{
    public class Document
    {
        public Hdr Hdr { get; set; }
        public List<Str> StrList { get; set; }
        public List<StrInfo> StrInfoList { get; set; }
    }
}
