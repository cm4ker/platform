using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.LanguageServer.Protocol
{
    [JsonObject]
    internal struct MarkedString
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
