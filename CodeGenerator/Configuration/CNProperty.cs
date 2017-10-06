using System;

namespace CodeGeneration.Configuration
{
    public class CNProperty : ConfigurationNode
    {
        public int Precision { get; set; }
        public int Length { get; set; }
        public Type Type { get; set; }
        public bool IsKey { get; set; }
    }
}