using System;
using Portable.Xaml.Markup;
using ZenPlatform.Configuration;

[assembly: XmlnsDefinition("http://zplatform.com/conf/entityComponent", "ZenPlatform.EntityComponent.Configuration")]
namespace ZenPlatform.EntityComponent
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