using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.ServerClientShared.Network
{
    public interface IMessagePackager
    {
        byte[] PackMessage(object message);
        IEnumerable<object> UnpackMessages(byte[] byteString);
    }
}
