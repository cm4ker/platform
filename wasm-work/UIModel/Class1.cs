using System;
using System.Linq;
using WebAssembly.Browser.DOM;

namespace UIModel
{
    
    
    
    /*
     
     Нам нужно решить следующие проблемы:
     
     
     1) Создание формы на сервере | Решается XML схемой (билдим там, передаем, запускаем)
     2) Code Behind: нам нужно делать следующее
     XML Form Representation + Some code represesntation
     
     
     public class MyForm
     {
        private Node Element;
        public void MyForm()
        {
            Init();
        }     
        
        public void Init()
        {
            // Get XML
            
            Element = Service.Interpret(xml, this);
        }
     }

     Interpret(xml , form)
     {
        // CreateElement.. var element = Node.....
        if(Events != null)
            if(xml.HasOnClick)
            {
                node.OnClick += Reflection.GetMethod(xml.OnClick.Name);
            }
     
     }
     
     
     <form> 
     
        <Element OnClick="Test">
     
     </form>
     
     [Client]
     void Test()
     {
         //SomeCode
     }
     
     [Server]
     {
        ///Some code
        
        //It will be wrapped to server call proc on the client
        //and translate as is to the server assembly     
     }
     
     
     */
    
    public abstract class Component
    {
        public virtual Node Element { get; }

        protected Component()
        {
        }
    }

    public class Binding
    {
        public string Path;
    }

    public class Text : Component
    {
        public Text(Document document)
        {
            _underlyingElement = document.CreateElement<HTMLInputElement>();
            _underlyingElement.Type = InputElementType.Text;
            _underlyingElement.OnClick += (sender, args) =>
            {
                ((HTMLInputElement) sender).Value = this.GetType().GetProperties().First().GetMethod.Name;
            };
        }

        private HTMLInputElement _underlyingElement;

        public override Node Element => _underlyingElement;

        public string Value { get; set; }
    }

    public class ComboBox : Component
    {
    }

    public class Label : Component
    {
    }

    public class List : Component
    {
    }

    public class Menu : Component
    {
    }

    public class Panel : Component
    {
    }

    public class Button : Component
    {
    }

    public class Grid : Component
    {
    }
}