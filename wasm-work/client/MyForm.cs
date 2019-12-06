using System;
using System.Data;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UIModel.XML;
using WebAssembly.Browser.DOM;

public class MyForm
{
    public MyForm()
    {
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
    private void Init()
    {
        var f = new Form();
        f.Childs.Add(new Button() {OnClick = "Test", Text = "Push me"});
        Service.Interpret(f, this);
    }

    private void Test(object sender, object args)
    {
        Console.WriteLine("Hello");
    }
}

public static class Service
{
    public static void Interpret(Form form, object instance)
    {
        var doc = new Document();

        foreach (var child in form.Childs)
        {
            if (child is Button b)
            {
                var htmlButton = doc.CreateElement<HTMLInputElement>();
                htmlButton.Type = InputElementType.Button;

                if (!string.IsNullOrEmpty(b.OnClick))
                {
                    htmlButton.OnClick += (sender, args) =>
                    {
                        var method = instance.GetType().GetMethod(b.OnClick,
                                         BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ??
                                     throw new Exception("The method not found");

                        // Тут мы можем обернуть ещё это всё
                        method.Invoke(instance, new object[] {sender, args});
                    };
                }

                if (!string.IsNullOrEmpty(b.Text))
                {
                    htmlButton.Value = b.Text;
                }

                doc.Body.AppendChild(htmlButton);
            }
        }
    }
}