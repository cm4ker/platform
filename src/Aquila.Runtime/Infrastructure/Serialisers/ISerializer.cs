﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.Serialisers
{
    public interface ISerializer
    {
        byte[] ToBytes<T>(T input);

        T FromBytes<T>(byte[] input);

        object FromBytes(byte[] input);
    }
}