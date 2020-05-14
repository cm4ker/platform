using System;
using System.Collections.Generic;
using Portable.Xaml;
using Portable.Xaml.Schema;
using Aquila.Core.Contracts;
using Aquila.Core.Network;

namespace Aquila.ServerRuntime
{
    public class MyXamlType : XamlType
    {
        public MyXamlType(Type underlyingType, XamlSchemaContext schemaContext) : base(underlyingType, schemaContext)
        {
        }

        public MyXamlType(Type underlyingType, XamlSchemaContext schemaContext, XamlTypeInvoker invoker) : base(
            underlyingType, schemaContext, invoker)
        {
        }

        public MyXamlType(string unknownTypeNamespace, string unknownTypeName, IList<XamlType> typeArguments,
            XamlSchemaContext schemaContext) : base(unknownTypeNamespace, unknownTypeName, typeArguments, schemaContext)
        {
        }

        protected MyXamlType(string typeName, IList<XamlType> typeArguments, XamlSchemaContext schemaContext) : base(
            typeName, typeArguments, schemaContext)
        {
        }

        protected override bool LookupIsPublic()
        {
            return true;

            return base.LookupIsPublic();
        }
    }
}