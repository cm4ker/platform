﻿using System;

namespace ZenPlatform.UIBuilder.Interface
{
    public class UIGroup : UINode
    {
        public UIGroupOrientation Orientation { get; set; }


        public UIGroup With(UINode node)
        {
            Childs.Add(node);
            return this;
        }

        public UIGroup With(Func<UIFactory, UINode> factory)
        {
            return With(factory(UIFactory.Get()));
        }
    }
}