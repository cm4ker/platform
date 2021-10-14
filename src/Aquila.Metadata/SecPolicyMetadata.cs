using System.Collections.Generic;

namespace Aquila.Metadata
{
    public sealed class SecMetadata
    {
        public string SecId { get; set; }

        public List<string> Subjectcs { get; set; }

        public List<(string Right, string Query)> Permissions { get; set; }
    }

    public sealed class SecSetMetadata
    {
        public List<string> Secs { get; set; }
    }
}