﻿using System;
using ZenPlatform.Configuration;

namespace ZenPlatform.EntityComponent
{
    public class Info : XCComponentInformation
    {
        public override string ComponentName => "Document";
        public override Guid ComponentId => Guid.Parse("230c6759-ae4e-408f-94b9-798749333f07");
        public override string ComponentSpaceName { get; } = "Document";
    }
}