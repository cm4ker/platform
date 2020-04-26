using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia.Data;
using UIModel;
using UIModel.XML;
using WebAssembly.Browser.DOM;
using Button = UIModel.XML.Button;
using Container = UIModel.XML.Container;

public static class Service
{
    public static Document Doc { get; } = new Document();

    public class RenderParameters
    {
        public RenderParameters(HTMLElement layer, object instance, object dataContext, List<ContextObject> objects)
        {
            Layer = layer;
            Instance = instance;
            DataContext = dataContext;
            Objects = objects;
        }

        public HTMLElement Layer { get; }
        public object Instance { get; }
        public object DataContext { get; }

        public List<ContextObject> Objects { get; }
    }

    public static void Interpret(Control control, RenderParameters p)
    {
        var dict = new Dictionary<Type, Func<Control, RenderParameters, HTMLElement>>();
        dict[typeof(Button)] = RenderButton;
        dict[typeof(Container)] = RenderContainer;
        dict[typeof(Field)] = RenderField;

        Type renderType = control.GetType();

        HTMLElement element = null;

        while (renderType != null && renderType != typeof(object))
        {
            try
            {
                element = dict[renderType](control, p);
                break;
            }
            catch (KeyNotFoundException ex)
            {
                renderType = renderType.BaseType;
            }
        }

        if (element == null)
            throw new Exception($"Can't render type {control.GetType()}");

        p.Layer.AppendChild(element);
    }

    private static HTMLElement RenderContainer(Control md, RenderParameters p)
    {
        var cont = md as Container;

        var contLayer = Doc.CreateElement<HTMLDivElement>();

        foreach (var item in cont.Controls)
            Interpret(item, new RenderParameters(contLayer, p.Instance, p.DataContext, p.Objects));

        return contLayer;
    }

    private static HTMLElement RenderField(Control md, RenderParameters p)
    {
        var fieldMD = md as Field ?? throw new Exception($"Render field only for render field ({md.GetType()})");
        var htmlField = Doc.CreateElement<HTMLInputElement>();

        htmlField.Type = fieldMD.Type switch
        {
            FieldType.Date => InputElementType.Date,
            FieldType.Text => InputElementType.Text,
            FieldType.Integer => InputElementType.Number
        };

        if (!string.IsNullOrEmpty(fieldMD.DefaultValue))
        {
            htmlField.Value = fieldMD.DefaultValue;
        }

        foreach (var binding in fieldMD.Bindings)
        {
            Console.WriteLine("Binding path=" + binding.Path);

            var b = new Avalonia.Data.Binding();
            b.Path = "A";
            b.Source = p.DataContext;
            b.Mode = BindingMode.TwoWay;
            
            
            var bind = new Bind(p.DataContext,
                (inst, value) => inst.Set(binding.Path, value),
                (inst) => inst.Get(binding.Path));

            //ComputePath(binding.Path,p.DataContext.GetType());
            htmlField.OnChange += (sender, args) =>
            {
                bind.BSet(((HTMLInputElement) sender).Value);
                Console.WriteLine(((MyForm.TestEntity) p.DataContext).A);
            };

            Console.WriteLine(bind.BGet());
            //Document.Data

            /*
             Путь может быть следующим выражением через точку
             Invoice.Contract.Contractor.Name - Auto One Way
             
             Invoice.Contract - Auto Two way
             
             =>
             
             Свойство Contract имеет следующую стркутуру
             
                Contract = {
                    //Field for to string impl
                    Name = ...
                
                    Contractor = {
                        Name
                    } 
                } 
             
             */
        }


        return htmlField;
    }

    private static HTMLElement RenderButton(Control md, RenderParameters p)
    {
        var b = md as Button;
        var htmlButton = Doc.CreateElement<HTMLInputElement>();
        htmlButton.Type = InputElementType.Button;

        if (!string.IsNullOrEmpty(b.OnClick))
        {
            htmlButton.OnClick += (sender, args) =>
            {
                var method = p.Instance.GetType().GetMethod(b.OnClick,
                                 BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ??
                             throw new Exception("The method not found");

                // Тут мы можем обернуть ещё это всё
                method.Invoke(p.Instance, new object[] {sender, args});
            };
        }

        if (!string.IsNullOrEmpty(b.Text))
        {
            htmlButton.Value = b.Text;
        }

        return htmlButton;
    }
}