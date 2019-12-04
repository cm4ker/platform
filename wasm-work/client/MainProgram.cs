using System;
using System.Runtime.CompilerServices;
using WebAssembly;
using WebAssembly.Browser.DOM;

public class Program
{
    public static void Main()
    {
        var document = new Document();
             
        // Create a new div element
        var newDiv = document.CreateElement<HTMLDivElement>();

        // and give it some content
        var newContent = document.CreateTextNode("Hi there and greetings!");
             
        // add the text node to the newly created div
        newDiv.AppendChild(newContent);

        // add the newly created element and its content into the DOM 
        var currentDiv = document.GetElementById("div1"); 

        document.Body.InsertBefore(newDiv, currentDiv); 
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