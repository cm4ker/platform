using System;
using System.IO;
using System.Xml.Serialization;
using UIModel.XML;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var xml = @"
<Form>
     <Data> 
        <Object Type='TestEntity' Name='Obj'></Object> <!-- Type property is optional -->
     </Data>
     <Controls>         
        <Button OnClick='Test' Text='Push me'/> 
        <Button OnClick='Test' Text='Push me'/>
        <Field  Type='Text' DefaultValue='Hello'>
            <Bindings>
                <Binding Property='Value' Path='A'/> 
            </Bindings>
        </Field>
     </Controls>
 </Form>
";

            XmlSerializer x = new XmlSerializer(typeof(Form));

            StringReader sr = new StringReader(xml);

            var f = (Form) x.Deserialize(sr);
            sr.Dispose();

        }
    }
}