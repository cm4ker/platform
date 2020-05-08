using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Network
{
    public interface IMessagePackager
    {
        byte[] PackMessage(object message);
        IEnumerable<object> UnpackMessages(byte[] byteString);
    }
}
