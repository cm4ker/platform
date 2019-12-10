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
     <Controls>         
        <Button OnClick='Test' Text='Push me'/> 
        <Button OnClick='Test' Text='Push me'/>
        <Field  Type='Text' Value='Push me'/>
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