using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UIModel.XML;
using WebAssembly;
using WebAssembly.Browser.DOM;


public static class ObjectExt
{
    public static object Get(this object obj, string propName)
    {
        Console.WriteLine(obj.GetType());
        
        var prop = obj.GetType().GetProperty(propName);
        if (prop == null)
            throw new Exception("Property not found");

        return prop.GetMethod.Invoke(obj, null);
    }

    public static void Set(this object obj, string propName, object value)
    {
        var prop = obj.GetType().GetProperty(propName);
        if (prop == null)
            throw new Exception("Property not found");

        var propType = prop.PropertyType;

        prop.SetMethod.Invoke(obj, new[] {value});
    }
}

public class Bind
{
    private readonly object _instance;
    private readonly Action<object, object> _setter;
    private readonly Func<object, object> _getter;

    public Bind(object instance, Action<object, object> setter, Func<object, object> getter)
    {
        _instance = instance;
        _setter = setter;
        _getter = getter;
    }
    
    

    public object BGet()
    {
        return _getter(_instance);
    }

    public void BSet(object obj)
    {
        _setter(_instance, obj);
    }
}

public class MyForm
{
    public class TestEntity
    {
        public string A { get; set; } = "";
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

        //get object from server
        var test = new TestEntity();

        var doc = new Document();

        var formLayer = doc.CreateElement<HTMLDivElement>();

        Service.Interpret(f, new Service.RenderParameters(layer, test, test, f.ContextObject));

        layer.AppendChild(formLayer);
    }

    private void Test(object sender, object args)
    {
        Console.WriteLine("Hello");
    }
}