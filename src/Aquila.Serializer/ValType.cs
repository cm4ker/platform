using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Serializer
{
    public enum ValType
    {
        Unknown = 0,
        String = 1,
        Int = 2,
        Char = 3,
        Boolean = 4,
        Long = 5,
        ByteArray = 6,
        Datetime = 7,
        DtoObject = 8,
        UXContainer = 9
    }
}