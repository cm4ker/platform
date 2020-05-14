using System;
using Portable.Xaml.Markup;
using Aquila.Configuration;
using Aquila.Configuration.Common.TypeSystem;

[assembly: XmlnsDefinition("http://zplatform.com/conf/entityComponent", "Aquila.EntityComponent.Configuration")]

namespace Aquila.EntityComponent
{
    /// <inheritdoc />
    public class Info : XCComponentInformation
    {
        /// <inheritdoc />
        public override string ComponentName => "Entity";

        /// <inheritdoc />
        public override Guid ComponentId => Guid.Parse("230c6759-ae4e-408f-94b9-798749333f07");

        /// <inheritdoc />
        public override string ComponentSpaceName { get; } = "Entity";
    }
}