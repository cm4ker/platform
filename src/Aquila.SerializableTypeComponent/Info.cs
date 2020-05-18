using System;
using Aquila.Configuration.Common.TypeSystem;
using Portable.Xaml.Markup;

[assembly:
    XmlnsDefinition("http://zplatform.com/conf/serializableTypesComponent",
        "Aquila.SerializableTypeComponent.Configuration")]

namespace Aquila.SerializableTypeComponent
{
    /// <inheritdoc />
    public class Info : XCComponentInformation
    {
        /// <inheritdoc />
        public override string ComponentName => "SerializableType";

        /// <inheritdoc />
        public override Guid ComponentId => Guid.Parse("2592EED3-F1B9-4F7C-A182-745DDE82CCE5");

        /// <inheritdoc />
        public override string ComponentSpaceName { get; } = "SerializableType";
    }
}