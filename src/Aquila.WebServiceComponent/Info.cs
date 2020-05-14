using System;
using Aquila.Configuration.Common.TypeSystem;
using Avalonia.Metadata;

[assembly:
    XmlnsDefinition("http://zplatform.com/conf/webServiceComponent",
        "Aquila.WebServiceComponent.Configuration")]

namespace Aquila.WebServiceComponent
{
    /// <inheritdoc />
    public class Info : XCComponentInformation
    {
        /// <inheritdoc />
        public override string ComponentName => "WebService";

        /// <inheritdoc />
        public override Guid ComponentId => Guid.Parse("1D511BB6-80C2-4DF6-9142-3F6EA4C6D8DE");

        /// <inheritdoc />
        public override string ComponentSpaceName { get; } = "WebService";
    }
}