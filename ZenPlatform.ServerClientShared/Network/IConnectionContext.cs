﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network
{
    public interface IConnectionContext
    {
        IConnection Connection { get; }
    }

}