using System;

namespace ZenPlatform
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public object ComplexProperty { get; set; }

        public int ComplexPropertyCurrentType => 1;

        public override string? ToString()
        {
            return "this is my invoice";
        }
    }
}