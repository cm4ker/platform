﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquila.LanguageServer.Protocol
{
    [JsonObject]
    internal class TextDocumentIdentifier
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
