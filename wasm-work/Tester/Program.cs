using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Data;
using UIModel.HtmlWrapper;
using UIModel.XML;

namespace Tester
{
    public class DModel
    {
        public object Test { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            UIContainer ui = new UIContainer();
            var obj = new {Test = "OPA!!"};
            ui.DataContext = obj;

            ObjectPickerField p = new ObjectPickerField();

            Binding b = new Binding();
            b.Mode = BindingMode.TwoWay;
            b.Path = "Test";
            
            var ib = b.Initiate(p, ObjectPickerField.ValueProperty, ui);

            var a = BindingOperations.Apply(p, ObjectPickerField.ValueProperty, ib, ui);
            
            Console.WriteLine(p.Value);
            
            p.Value = "changed!";
            
            Console.WriteLine(obj.Test);
        }
    }
}