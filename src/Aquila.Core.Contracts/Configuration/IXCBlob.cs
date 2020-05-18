using System;

namespace Aquila.Core.Contracts.Configuration
{
    public interface IXCBlob
    {
        string Name { get; set; }

        Uri URI { get; set; }

        string Hash { get; set; }
    }
}