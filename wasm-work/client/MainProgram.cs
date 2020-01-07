﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Data;
using UIModel.HtmlWrapper;
using WebAssembly;
using WebAssembly.Browser.DOM;

public class Program
{
    public class DModel
    {
        public object Test { get; set; }
    }

    public static void MainOLD()
    {
        var document = new Document();
        var newDiv = document.CreateElement<HTMLDivElement>();

        var newContent = document.CreateTextNode("Хеллоу из .net'a!");
        newDiv.AppendChild(newContent);
        var currentDiv = document.GetElementById("div1");
        document.Body.InsertBefore(newDiv, currentDiv);

        var text = new UIModel.Text(document);

        document.Body.AppendChild(text.Element);

        MyExample(document.Body);

        UIContainer ui = new UIContainer();
        var obj = new DModel {Test = "OPA!!"};
        ui.DataContext = obj;

        ObjectPickerField op = new ObjectPickerField();

        op.InitAutocomplete(new List<string> {"Apple", "Test", "Test1", "Test2", "Lol"});

        ObjectPickerField op2 = new ObjectPickerField();

        op2.InitAutocomplete(new List<string> {"AAAA", "BBBB", "CCCC", "DDDD", "EEEE"});

        Binding b = new Binding();
        b.Mode = BindingMode.TwoWay;
        b.Path = "Test";

        var ib = b.Initiate(op, ObjectPickerField.ValueProperty, ui);
        BindingOperations.Apply(op, ObjectPickerField.ValueProperty, ib, ui);

        var ib2 = b.Initiate(op2, ObjectPickerField.ValueProperty, ui);
        BindingOperations.Apply(op2, ObjectPickerField.ValueProperty, ib, ui);

        document.Body.AppendChild(op.Root);
        document.Body.AppendChild(op2.Root);


        /*
         
            Form 
                .Add(Params componetnts)
                
            Grid
                .
            
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

    private static int pos = 0;

    public static Grid G { get; set; }

    public static void Main3()
    {
        G = new Grid();
        G.Init();
        D.Doc.Body.AppendChild(G.Root);

        var b = D.Doc.CreateElement<HTMLInputElement>();
        b.Type = InputElementType.Button;

        b.OnClick += (sender, args) =>
        {
            pos++;
            G.Scroll(pos);
        };

        D.Doc.Body.AppendChild(b);

        Console.WriteLine("DOCUMENT: " + D.Doc.IsCorrupted);

        GC.Collect();
        GC.WaitForPendingFinalizers();

//        Task.Delay(2000).Wait();

        Console.WriteLine("DOCUMENT AFTER: " + D.Doc.IsCorrupted);
    }

    public static HTMLDivElement A;

    public static void Main()
    {
        A = D.Doc.CreateElement<HTMLDivElement>();

        A.Bug = "Object A";

        var b = D.Doc.CreateElement<HTMLDivElement>();
        b.Bug = "Object B";

        A.AppendChild(b);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine(A.IsCorrupted);
    }


    private static void MyExample(HTMLElement layer)
    {
        MyForm f = new MyForm(layer, null);
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