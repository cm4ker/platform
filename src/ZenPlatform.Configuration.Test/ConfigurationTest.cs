using Xunit;
using ZenPlatform.Configuration.Structure;
using SharpFileSystem.FileSystems;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Storage;
using ZenPlatform.Test.Tools;

namespace ZenPlatform.Configuration.Test
{
    public class ConfigurationTest
    {
        [Fact]
        public void ExampleConfNotNull()
        {
            Assert.NotNull(ConfigurationFactory.Create());
        }

        [Fact]
        public void EqualsExampleCofiguration()
        {
            EqualsConfiguration(ConfigurationFactory.Create(), ConfigurationFactory.Create());
        }

        [Fact]
        public void SaveAndLoadCofigurationRepeatedly()
        {
            IProject expect = ConfigurationFactory.Create();
            IProject actual;
            var storage = new MemoryFileSystem();

            expect.Save(storage);
            actual = Project.Load(new MDManager(new TypeManager(), new InMemoryUniqueCounter()), storage);

            Assert.Equal(expect, actual);
        }

        [Fact]
        public void SaveAndLoadCofiguration()
        {
            var configuration = ConfigurationFactory.Create();

            var storage = new MemoryFileSystem();

            configuration.Save(storage);

            //
            // var xml1 = new StreamReader(new MemoryStream(storage.Blobs.First(f => f.Key == "Store").Value)).ReadToEnd();
            //
            // var loadedConfiguration = Structure.Project.Load(storage);
            //
            // EqualsConfiguration(configuration, loadedConfiguration);
        }

        [Fact]
        public void DatabaseStorage()
        {
            return;
            // var configuration = ConfigurationFactory.Create();
            //
            //
            //
            // using (var context = new DataContext(QueryBuilder.SqlDatabaseType.SqlServer, 
            //     "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = testdb; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False"))
            // {
            //
            //     var storage = new XCDatabaseStorage("test", context);
            //
            //     configuration.Save(storage);
            //
            //     var loadedConfiguration = Structure.Project.Load(storage);
            //
            //     EqualsConfiguration(configuration, loadedConfiguration);
            // }
        }


        [Fact]
        public void FileSystemStorage()
        {
            var configuration = ConfigurationFactory.Create();
            //
            // var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            // using (var storage = new XCFileSystemStorage(path, "Test"))
            // {
            //
            //     configuration.Save(storage);
            //
            //     var loadedConfiguration = Structure.Project.Load(storage);
            //
            //
            //
            //     EqualsConfiguration(configuration, loadedConfiguration);
            // }
            //
            // Directory.Delete(path, true);
        }


        private void EqualsConfiguration(IProject l, IProject r)
        {
            // var storage1 = new XCMemoryStorage();
            //
            // var storage2 = new XCMemoryStorage();
            //
            // l.Save(storage1);
            // r.Save(storage2);
            //
            // Assert.Equal(storage1.Blobs.Count, storage2.Blobs.Count);
            //
            // var join = storage1.Blobs.Join(storage2.Blobs, k => k.Key, k => k.Key, (l, r) => new { left = l, right = r });
            //
            // foreach (var item in join)
            // {
            //     var res = item.left.Value.SequenceEqual(item.right.Value);
            //     if (!res)
            //     {
            //         var xml1 = new StreamReader(new MemoryStream(item.left.Value)).ReadToEnd();
            //
            //         var xml2 = new StreamReader(new MemoryStream(item.right.Value)).ReadToEnd();
            //     }
            //     Assert.True(res, $"Not equals path = {item.left.Key}");
            //
            // }
        }
    }
}