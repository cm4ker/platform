using System.IO;
using Xunit;
using ZenPlatform.Tests.XmlSerializer.TestedObjects;

namespace ZenPlatform.Tests.XmlSerializer
{
    public class SerializationTest
    {
        private ZenPlatform.XmlSerializer.XmlSerializer _serializer = new ZenPlatform.XmlSerializer.XmlSerializer();

        [Fact]
        public void SimpleObject()
        {
            var instance = new SimpleObjectWithNastedObject();
            using (var tw = new StringWriter())
            {
                {
                    _serializer.Serialize(instance, tw);
                    var result = tw.ToString();
                }
            }
        }
    }