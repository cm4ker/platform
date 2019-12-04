using System;
using System.Runtime.CompilerServices;
using WebAssembly;
using WebAssembly.Browser.DOM;

public class Program
{
    public static void Main()
    {
        var document = new Document();
        var newDiv = document.CreateElement<HTMLDivElement>();
        
        var newContent = document.CreateTextNode("Hi there and greetings!");
        newDiv.AppendChild(newContent);
        var currentDiv = document.GetElementById("div1");
        document.Body.InsertBefore(newDiv, currentDiv);

        /*
         
            Form
                .Add(Params componetnts)
                
            Grid
            
            Button
            
            RadioButton
            
            ComboBox
            
            TextBox
            
            
         
            1) Server construct from in intermediate language
            2) Server send serialized form to client
            3) Client recieve from
            4) Client starting interpretate intermediate language and construct from
            5) Client end construct form and show it
        */
    }
}

public static class C
{
    public static int Add(int a, int b)
    {
        var g = (JSObject) Runtime.GetGlobalObject("document");

        if (g == null)
            throw new Exception("Это фиаско, братан!");

        var c = (int) g.GetObjectProperty("test");

        return a + b + c;
    }
}