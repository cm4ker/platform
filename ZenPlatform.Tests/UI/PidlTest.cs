using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Pidl;

namespace ZenPlatform.Tests.UI
{
    [TestClass]
    public class PidlTest
    {
        [TestMethod]
        public void TestPidlReader()
        {
            using (var reader = new PIDLReader())
            {
                reader.Read();
            }
        }
    }
}