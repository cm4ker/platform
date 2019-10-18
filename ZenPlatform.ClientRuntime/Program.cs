﻿using System;
using ZenPlatform.Client;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Network.Contracts;

namespace ZenPlatform.ClientRuntime
{
    /// <summary>
    /// Оснавная точка входа в программу
    /// </summary>
    public class Infrastructure
    {
        public static void Main(IPlatformClient client)
        {
            GlobalScope.Client = client;
        }
    }
}