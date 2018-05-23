using System;

namespace ZenPlatform.UIGeneration2 {
    public interface IControlFactory {

        String MakeControl(Int32? parentGridColumn = null, Int32? parentGridRow = null);

    }
}