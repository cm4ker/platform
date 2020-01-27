using System;

namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface IObjectSetting
    {
        Guid ObjectId { get; set; }
        uint SystemId { get; set; }
        string DatabaseName { get; set; }
    }
}