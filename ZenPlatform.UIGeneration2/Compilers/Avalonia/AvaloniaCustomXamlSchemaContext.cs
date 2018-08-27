﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Context;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.PortableXaml;
using Avalonia.Markup.Xaml.Styling;
using Portable.Xaml;

namespace ZenPlatform.UIBuilder.Compilers.Avalonia
{
    /// <summary>
    /// Контекст схемы данных. Необходим для того, чтобы связать  реальные типы с типами XAML
    /// </summary>
    public class AvaloniaCustomXamlSchemaContext : XamlSchemaContext
    {
        private readonly IRuntimeTypeProvider _provider;

        public AvaloniaCustomXamlSchemaContext(IRuntimeTypeProvider provider)
        {
            _provider = provider;
        }

        private IRuntimeTypeProvider _avaloniaTypeProvider;

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

        private XamlType ResolveXamlTypeName(string xmlNamespace, string xmlLocalName, XamlType[] typeArguments, bool required)
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
            Type type = _avaloniaTypeProvider.FindType(xmlNamespace,
                                                        xmlLocalName,
                                                        genArgs) ??
                        _avaloniaTypeProvider.FindType(xmlNamespace,
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
            { typeof(Binding), typeof(BindingExtension) },
            { typeof(StyleInclude), typeof(StyleIncludeExtension) },
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
                return new BindingXamlType(type, this);
            }

            if (origType != null ||
                typeof(AvaloniaObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return new AvaloniaXamlType(type, this);
            }

            return null;
        }

        protected override Assembly OnAssemblyResolve(string assemblyName)
        {
            return base.OnAssemblyResolve(assemblyName);
        }

        public override IEnumerable<string> GetAllXamlNamespaces()
        {
            return base.GetAllXamlNamespaces();
        }

        public override bool TryGetCompatibleXamlNamespace(string xamlNamespace, out string compatibleNamespace)
        {
            return base.TryGetCompatibleXamlNamespace(xamlNamespace, out compatibleNamespace);
        }
    }
}