using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Styling;
using Portable.Xaml;
using Portable.Xaml.Markup;
using XmlnsDefinitionAttribute = Avalonia.Metadata.XmlnsDefinitionAttribute;

namespace ZenPlatform.UIBuilder.Compilers.Avalonia
{
    public class RuntimeTypeProvider
    {
        public Type FindType(string @namespace, string name, params Type[] args)
        {
            return null;
        }
    }

    /// <summary>
    /// Контекст схемы данных. Необходим для того, чтобы связать  реальные типы с типами XAML
    /// </summary>
    public class AvaloniaCustomXamlSchemaContext : XamlSchemaContext
    {
        private RuntimeTypeProvider _tp;

        public AvaloniaCustomXamlSchemaContext()
        {
            _tp = new RuntimeTypeProvider();
        }

        protected override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
        {
            XamlType type = null;
            try
            {
                type = ResolveXamlTypeName(xamlNamespace, name, typeArguments, false);

                if (type == null)
                {
                    type = base.GetXamlType(xamlNamespace, name, typeArguments);
                }
            }
            catch (Exception e)
            {
                //TODO: log or wrap exception
                throw e;
            }

            return type;
        }

        private XamlType ResolveXamlTypeName(string xmlNamespace, string xmlLocalName, XamlType[] typeArguments,
            bool required)
        {
            Type[] genArgs = null;
            if (typeArguments != null && typeArguments.Any())
            {
                genArgs = typeArguments.Select(t => t?.UnderlyingType).ToArray();

                if (genArgs.Any(t => t == null))
                {
                    return null;
                }
            }

            // MarkupExtension type could omit "Extension" part in XML name.
            Type type = _tp.FindType(xmlNamespace,
                            xmlLocalName,
                            genArgs) ??
                        _tp.FindType(xmlNamespace,
                            xmlLocalName + "Extension",
                            genArgs);

            if (type != null)
            {
                Type extType;
                if (_wellKnownExtensionTypes.TryGetValue(type, out extType))
                {
                    type = extType;
                }
            }

            if (type == null)
            {
                //let's try the simple types
                //in Portable xaml like xmlns:sys='clr-namespace:System;assembly=mscorlib'
                //and sys:Double is not resolved properly

                return base.GetXamlType(xmlNamespace, xmlLocalName);
            }

            return GetXamlType(type);
        }


        public override XamlType GetXamlType(Type type)
        {
            //TODO: Нужно просканировать типы, которые у нас есть и вернуть кастомно собранный

            /*
             * Подробное описание задачи таково:
             * У нас есть тип - typeof(DataGrid) мы должны создать список типов и пространств xml имён.
             * В этот список все имена xmlns затягиваются с участием атрибута XmlnsDefinitionAttribute
             * Т.е, если  есть  это обозначение, значит мы затягиваем все типы с сопоставлением кастомного namespace.
             *
             * Все типы необходимо получать  именно через контекст, а не на прямую, как сделано сейчас
             *
             * 
             */

            return GetAvaloniaXamlType(type) ?? base.GetXamlType(type);
        }

        private static readonly Dictionary<Type, Type> _wellKnownExtensionTypes = new Dictionary<Type, Type>()
        {
            {typeof(Binding), typeof(BindingExtension)},
            {typeof(StyleInclude), typeof(StyleIncludeExtension)},
        };

        /*
         * Если у типа уже есть аттрибут XmlnsDefinition на сборке, в таком случае необходимо использовать пространство имён из него
         */

        //TODO: Имплементировать кастомное получение пространства имён

        private XamlType GetAvaloniaXamlType(Type type)
        {
            //if type is extension get the original type to check
            var origType = _wellKnownExtensionTypes.FirstOrDefault(v => v.Value == type).Key;

            if (typeof(IBinding).GetTypeInfo().IsAssignableFrom((origType ?? type).GetTypeInfo()))
            {
                //return new BindingXamlType(type, this);

                return null;
            }

            if (origType != null ||
                typeof(AvaloniaObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return new MyXamlType(type, this);
            }

            return null;
        }

        public override ICollection<XamlType> GetAllXamlTypes(string xamlNamespace)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var list = new List<XamlType>();

            foreach (var asm in asms)
            {
                foreach (var attr in asm.GetCustomAttributes<XmlnsDefinitionAttribute>())
                {
                    foreach (var t in asm.GetExportedTypes())
                        if (t.Namespace == attr.ClrNamespace && !t.GetTypeInfo().IsNested)
                            list.Add(GetXamlType(t));
                }
            }

            return list;
        }
    }
}