using System;

namespace Aquila.Core.Infrastructure.Settings;

public class ConfigFileNameAttribute : Attribute
{
    public string Name { get; set; }
}