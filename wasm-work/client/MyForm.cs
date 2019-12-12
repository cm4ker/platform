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
    class TestEntity
    {
        public string A { get; set; }

        public DateTime B { get; set; }
    }

    private readonly HTMLElement _layer;
    private object _dataContext;


    public MyForm(HTMLElement layer, object dataContext)
    {
        _layer = layer;
        _dataContext = dataContext;

        Init(_layer, dataContext);
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
     
     MasterObject
        - Obj
        - Obj2
        ...
     
     */


    private void Init(HTMLElement layer, object dataContext)
    {
        var xml = @"
 <Form>
     <Data> 
        <Object Type='TestEntity' Name='Obj'></Object> <!-- Type property is optional -->
     </Data>
     <Controls>         
        <Button OnClick='Test' Text='Push me'/> 
        <Button OnClick='Test' Text='Push me'/>
        <Field  Type='Date' DefaultValue='2019-01-01'>
            <Bindings>
                <Binding Property='Value' Path='Obj.B'/> 
            </Bindings>
        </Field>
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

        layer.AppendChild(formLayer);
    }

    private void Test(object sender, object args)
    {
        Console.WriteLine("Hello");
    }
}