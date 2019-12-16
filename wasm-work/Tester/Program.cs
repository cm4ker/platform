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
    /*
        
        Dto
        {
            Version
            Type   
            Guid
            String
            Int
             
        }
                
        ModelClient
        {
            Link = LinkAnother     
        }
                 
     
     */


    public class DModel
    {
        public object Test { get; set; }
    }


    public class Link
    {
        public object Type
        {
            get
            {
                //Return RequestServerForData
                return null;
            }
        }
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

            BindingOperations.Apply(p, ObjectPickerField.ValueProperty, ib, ui);

            Console.WriteLine(p.Value);

            p.Value = "changed!";

            Console.WriteLine(obj.Test);
        }
    }
}