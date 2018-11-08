using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Portable.Xaml;
using Portable.Xaml.Schema;

namespace ZenPlatform.Configuration.Structure
{
    public class XCXamlSchemaContext : XamlSchemaContext
    {
        protected override XamlMember GetProperty(PropertyInfo propertyInfo)
        {
            return new XCXamlMember(propertyInfo, this);
        }
    }

    public class XCXamlMember : XamlMember
    {
        public XCXamlMember(EventInfo eventInfo, XamlSchemaContext schemaContext) : base(eventInfo, schemaContext)
        {
        }

        public XCXamlMember(EventInfo eventInfo, XamlSchemaContext schemaContext, XamlMemberInvoker invoker) : base(
            eventInfo, schemaContext, invoker)
        {
        }

        public XCXamlMember(PropertyInfo propertyInfo, XamlSchemaContext schemaContext) : base(propertyInfo,
            schemaContext)
        {
        }

        public XCXamlMember(PropertyInfo propertyInfo, XamlSchemaContext schemaContext, XamlMemberInvoker invoker) :
            base(propertyInfo, schemaContext, invoker)
        {
        }

        public XCXamlMember(string attachableEventName, MethodInfo adder, XamlSchemaContext schemaContext) : base(
            attachableEventName, adder, schemaContext)
        {
        }

        public XCXamlMember(string attachableEventName, MethodInfo adder, XamlSchemaContext schemaContext,
            XamlMemberInvoker invoker) : base(attachableEventName, adder, schemaContext, invoker)
        {
        }

        public XCXamlMember(string attachablePropertyName, MethodInfo getter, MethodInfo setter,
            XamlSchemaContext schemaContext) : base(attachablePropertyName, getter, setter, schemaContext)
        {
        }

        public XCXamlMember(string attachablePropertyName, MethodInfo getter, MethodInfo setter,
            XamlSchemaContext schemaContext, XamlMemberInvoker invoker) : base(attachablePropertyName, getter, setter,
            schemaContext, invoker)
        {
        }

        public XCXamlMember(ParameterInfo parameterInfo, XamlSchemaContext schemaContext) : base(parameterInfo,
            schemaContext)
        {
        }

        public XCXamlMember(ParameterInfo parameterInfo, XamlSchemaContext schemaContext, XamlMemberInvoker invoker) :
            base(parameterInfo, schemaContext, invoker)
        {
        }

        public XCXamlMember(string name, XamlType declaringType, bool isAttachable) : base(name, declaringType,
            isAttachable)
        {
        }

        protected override bool LookupIsReadPublic()
        {
           
            var getter = LookupUnderlyingGetter();
            if (getter != null)
            {
                var attr = LookupUnderlyingMember().GetCustomAttribute<XCDecoratedForSerializationAttribute>();
                if (attr != null)
                {
                    return true;
                }
            }

            return base.LookupIsReadPublic();
        }

        protected override bool LookupIsWritePublic()
        {
            var setter = LookupUnderlyingSetter();
            if (setter != null)
            {
                var attr = LookupUnderlyingMember().GetCustomAttribute<XCDecoratedForSerializationAttribute>();
                if (attr != null)
                {
                    return true;
                }
            }

            return base.LookupIsWritePublic();
        }
    }
}