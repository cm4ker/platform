using System;
using System.Dynamic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UIModel.XML;
using WebAssembly;
using WebAssembly.Browser.DOM;

public class MyForm
{
    private readonly HTMLElement _layer;


    public MyForm(HTMLElement layer)
    {
        _layer = layer;
        Init();
    }

    /*
     
     <Form>
        <Form.Childs>
            <Button Onclick = "Test" Text="Push me"/> 
        </Form.Childs>
     </Form>
     
     
     function Test()
     {
        Message("Hello!!");     
        
        Document.Invoice.Create();
        
        List<DateTime> q = $$ FROM Invoice SELECT Data $$;
     }
     
     */

    class TestEntity()
    {
        public string A { get; set; }

        public DateTime B { get; set; }
    }

    private void Init()
    {
        var xml = @"
 <Form>
     <Controls>         
        <Button OnClick='Test' Text='Push me'/> 
        <Button OnClick='Test' Text='Push me'/>
        <Field  Type='Date' Value='Push me'/>
     </Controls>
 </Form>
";

        XmlSerializer x = new XmlSerializer(typeof(Form));

        StringReader sr = new StringReader(xml);

        var f = (Form) x.Deserialize(sr);
        sr.Dispose();

        //get object from server
        var test = new TestEntity();

        var doc = new Document();

        var formLayer = doc.CreateElement<HTMLDivElement>();

        Service.Interpret(f, formLayer, this, test);

        _layer.AppendChild(formLayer);
    }

    private void Test(object sender, object args)
    {
        Console.WriteLine("Hello");
    }
}