using System;
using Xunit;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Core;

namespace ZenPlatform.Component.Tests
{
    public class ViewBagTest
    {
        [Fact]
        void SimpleTest()
        {
            var vb = new ViewBag {{"Id", Guid.Parse("8b888935-895d-4806-beaf-0f9e9217ad1b")}, {"Type", 10}};

            InvoiceLink il = new InvoiceLink(vb);

            Assert.Equal("Entity = (10:{8b888935-895d-4806-beaf-0f9e9217ad1b})", il.Presentation);
        }
    }
}