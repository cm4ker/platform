using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIModel.XML;
using WebAssembly.Browser.DOM;

public static class Service
{
    public static Document Doc { get; } = new Document();

    public static void Interpret(Control control, HTMLElement layer, object instance, object dataContext = null)
    {
        var dict = new Dictionary<Type, Func<HTMLElement, Control, object, object, HTMLElement>>();
        dict[typeof(Button)] = RenderButton;
        dict[typeof(Container)] = RenderContainer;
        dict[typeof(Field)] = RenderField;

        Type renderType = control.GetType();

        Console.WriteLine(renderType);
        Console.WriteLine(renderType.BaseType);
        Console.WriteLine(renderType.BaseType?.BaseType);

        HTMLElement element = null;

        var maxDepth = 8;
        while (renderType != null && renderType != typeof(object) && maxDepth > 0)
        {
            try
            {
                Console.WriteLine(renderType);
                element = dict[renderType](layer, control, instance, dataContext);
                Console.WriteLine("Breaking");
                break;
            }
            catch
            {
                Console.WriteLine($"Change type to: {renderType.BaseType}");
                renderType = renderType.BaseType;
            }

            maxDepth--;
            Console.WriteLine($"Depth is");
        }

        if (element == null)
            throw new Exception($"Can't render type {control.GetType()}");

        layer.AppendChild(element);
    }

    private static HTMLElement RenderContainer(HTMLElement layer, Control md, object instance, object dataContext)
    {
        var cont = md as Container;

        var contLayer = Doc.CreateElement<HTMLDivElement>();

        foreach (var item in cont.Controls)
            Interpret(item, contLayer, instance);

        return contLayer;
    }

    private static HTMLElement RenderField(HTMLElement layer, Control md, object instance, object dataContext)
    {
        var fieldMD = md as Field ?? throw new Exception($"Render field only for render field ({md.GetType()})");
        var htmlField = Doc.CreateElement<HTMLInputElement>();

        htmlField.Type = fieldMD.Type switch
        {
            FieldType.Date => InputElementType.Date,
            FieldType.Text => InputElementType.Text,
            FieldType.Integer => InputElementType.Number
        };


        foreach (var binding in fieldMD.Bindings)
        {
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
            binding.Path
        }


        return htmlField;
    }

    private static HTMLElement RenderButton(HTMLElement layer, Control md, object instance, object dataContext)
    {
        var b = md as Button;
        var htmlButton = Doc.CreateElement<HTMLInputElement>();
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

        return htmlButton;
    }
}