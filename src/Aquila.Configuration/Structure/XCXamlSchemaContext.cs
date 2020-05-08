using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Portable.Xaml;

namespace Aquila.Configuration.Structure
{
    public class XCXamlSchemaContext : XamlSchemaContext
    {
        protected override XamlMember GetProperty(PropertyInfo propertyInfo)
        {
            return new XCXamlMember(propertyInfo, this);
        }
        
    }
}