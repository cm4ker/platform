using System;
using ZenPlatform.Configuration;

namespace ZenPlatform.DocumentComponent
{
    public class Info : ComponentInformation
    {
        public override string ComponentName => "Document";
        public override Guid ComponentId => Guid.Parse("230c6759-ae4e-408f-94b9-798749333f07");
        public override string ComponentSpaceName { get; } = "Document";
    }
}