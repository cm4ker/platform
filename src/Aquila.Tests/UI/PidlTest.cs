//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using Avalonia.Markup.Xaml.Context;
//using Avalonia.Markup.Xaml.PortableXaml;
//using MonoMac.Foundation;
//using Portable.Xaml;
//using Portable.Xaml.Markup;
//using Xunit;
//using Aquila.Pidl;
//using Aquila.Pidl.ObjectModel;
//using Aquila.Pidl.Xaml;
//
//namespace Aquila.Tests.UI
//{
//  
//    public class PidlTest
//    {
//        [Fact]
//        public void TestPidlReader()
//        {
//            Console.WriteLine("Start");
//            var container = new PlatformExtensionResolverImpl();
//
//            container.Register("Aquila", "Form", typeof(Form));
//            container.Register("Aquila", "Fragment", typeof(Fragment));
//            container.Register("Aquila", "Field", typeof(Field));
//            container.Register("Aquila", "FragmentRef", typeof(FragmentRef));
//            container.Register("Aquila", "Group", typeof(Group));
//
//            var sc = new PlatformXamlSchemaContext(container);
//
//
//            var pidl = @"
//<Form xmlns=""Aquila"">
//    <!-- Для фрагмента важно указать тип -->
//    <Fragment Id=""AB1A3649-53A6-4259-B746-6E7CEB79E7A3"" DataType=""AB1A3649-53A6-4259-B746-6E7CEB79E7A3"">
//        
//    </Fragment>
//
//    <!-- Помимо локальных фрагментов есть ещё глобальные фрагменты -->
//
//    <Group>
//       <Field Source=""ДатаДокумента"" />
//    </Group>
//
//    <FragmentRef Id=""AB1A3649-53A6-4259-B746-6E7CEB79E7A3"" /> <!-- А вот тут мы подключаем компоненет-->
//
//    <FragmentRef Id=""AB1A3649-53A6-4259-B746-6E7CEB79E7A3"" /> <!-- Мы можем ссылаться на 1 и тот же фрагмент сколько угодно раз-->
//</Form>
//";
//
//            TextReader tr = new StringReader(pidl);
//
//            using (var reader = new XamlXmlReader(tr, sc, null))
//            {
//                XamlObjectWriter writer = new XamlObjectWriter(sc);
//
//
//
//                XamlServices.Transform(reader, writer);
//
//                Console.WriteLine(writer.Result);
//
//                XamlObjectReader objReader = new XamlObjectReader(writer.Result);
//                var tw = new StringWriter();
//                XamlXmlWriter xmlWriter = new XamlXmlWriter(tw, sc);
//                XamlServices.Transform(objReader, xmlWriter);
//
//                Console.Write(tw.ToString());
//            }
//        }
//    }
//}

