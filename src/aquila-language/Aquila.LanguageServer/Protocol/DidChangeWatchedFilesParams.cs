﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquila.LanguageServer.Protocol
{
    [JsonObject]
    internal class DidChangeWatchedFilesParams
    {
        [JsonProperty("changes")]
        public FileEvent[] Changes { get; set; }
    }
}
