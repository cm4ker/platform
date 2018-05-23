using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Controls;

namespace ZenPlatform.IDE.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";

        public IList<Node> Tree { get; } = new Node().Children;
        
        public class Node
        {
            private IList<Node> _children;
            public string Header { get; private set; }
            public IList<Node> Children
            {
                get
                {
                    if (_children == null)
                    {
                        _children = Enumerable.Range(1, 10).Select(i => new Node() { Header = $"Item {i}" })
                            .ToArray();
                    }
                    return _children;
                }
            }
        }
    }
}
