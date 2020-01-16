﻿using System;


namespace ZenPlatform.Configuration.Contracts
{
    public interface ICommandEditor
    {
        IModuleEditor EditModule();
        ICommandEditor SetDisplayName(string name);
        ICommandEditor SetGuid(Guid guid);
        ICommandEditor SetName(string name);
    }
}