using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Xunit;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Tests.Common;


namespace ZenPlatform.Tests.Conf
{
    public class ConfSaveLoadTest
    {
        [Fact]
        public void SaveLoadTest()
        {
            var conf = Factory.CreateExampleConfiguration();
            var storage = new XCFileSystemStorage("./ExampleConfiguration", "TestProject.xml");
            conf.Save(storage);

            var fsStorage = new XCFileSystemStorage("./ExampleConfiguration", "TestProject.xml");

            var loadedConf = XCRoot.Load(fsStorage);

            Assert.Equal(conf.ProjectName, loadedConf.ProjectName);
            Assert.Equal(conf.ProjectId, loadedConf.ProjectId);
            Assert.Equal(conf.ProjectVersion, loadedConf.ProjectVersion);
            Assert.Equal(conf.Data.Components.Count, loadedConf.Data.Components.Count);
            Assert.Equal(conf.Data.ComponentTypes.Count(), loadedConf.Data.ComponentTypes.Count());
            Assert.Equal(conf.Data.ComponentTypes.First().GetProperties().Any(x => x.Unique),
                loadedConf.Data.ComponentTypes.First().GetProperties().Any(x => x.Unique));

            Assert.Equal(5,
                loadedConf.Data.ComponentTypes.FirstOrDefault(x => x.Name == "Invoice")
                    .GetPropertyByName("CompositeProperty").Types.Count);
        }
    }
}