using System;

namespace Aquila.Core.Contracts.TypeSystem
{
    /// <summary>
    /// Settings for objects (responce for store and sys info)
    /// </summary>
    public interface IObjectSetting
    {
        Guid ObjectId { get; set; }

        uint SystemId { get; set; }

        string DatabaseName { get; set; }
    }
}