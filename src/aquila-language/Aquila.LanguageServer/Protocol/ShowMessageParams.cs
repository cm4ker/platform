﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquila.LanguageServer.Protocol
{
    [JsonObject]
    internal class ShowMessageParams
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
