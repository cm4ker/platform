using System;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCBlob
    {
        string Name { get; set; }

        Uri URI { get; set; }

        string Hash { get; set; }
    }
}