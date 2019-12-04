﻿using System;
using WebAssembly;

namespace WebAssembly.Browser.DOM
{

    [Export("History", typeof(JSObject))]
    public sealed class History : DOMObject, IHistory
    {
        public History(JSObject handle) : base(handle) { }

        //public History() { }
        [Export("length")]
        public double Length => GetProperty<double>("length");
        [Export("state")]
        public Object State => GetProperty<Object>("state");
        [Export("scrollRestoration")]
        public ScrollRestoration ScrollRestoration { get => GetProperty<ScrollRestoration>("scrollRestoration"); set => SetProperty<ScrollRestoration>("scrollRestoration", value); }
        [Export("back")]
        public void Back()
        {
            InvokeMethod<object>("back");
        }
        [Export("forward")]
        public void Forward()
        {
            InvokeMethod<object>("forward");
        }
        [Export("go")]
        public void Go(double delta)
        {
            InvokeMethod<object>("go", delta);
        }
        [Export("pushState")]
        public void PushState(Object data, string title, string url)
        {
            InvokeMethod<object>("pushState", data, title, url);
        }
        [Export("replaceState")]
        public void ReplaceState(Object data, string title, string url)
        {
            InvokeMethod<object>("replaceState", data, title, url);
        }
    }

}