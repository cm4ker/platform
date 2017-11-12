using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.DataComponent;

namespace ZenPlatform.Tests
{
    [TestClass]
    public class StringNamedFormatterTest
    {
        [TestMethod]
        public void TestFormatString()
        {
            var formatted =
                "Test format this {SomeValue1} and this {SomeValue2}".NamedFormat(new
                {
                    SomeValue1 = "Hello",
                    SomeValue2 = "Hello2"
                });

            Assert.AreEqual("Test format this Hello and this Hello2", formatted);
        }
    }
}
