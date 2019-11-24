using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.ConfigurationExample;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Test.Configuration;
using ZenPlatform.Data;

namespace ZenPlatform.Core.Test
{
    public class ConfigurationStorageTest
    {
        [Fact]
        public void SaveAndLoadCofiguration()
        {
            var configuration = Factory.CreateExampleConfiguration();

            var storage = new XCTestMemoryStorage();

            configuration.Save(storage);

            var loadedConfiguration = XCRoot.Load(storage);

            Assert.Equal(configuration.GetHash(), loadedConfiguration.GetHash());
        }

        [Fact]
        public void DatabaseStorage()
        {
            var configuration = Factory.CreateExampleConfiguration();

            throw new NotSupportedException("нужно разобратся как сделать этот тест универсальным и не зависящим от connectionString");
            

            using (var context = new DataContext(QueryBuilder.SqlDatabaseType.SqlServer, ""))
            {

                var storage = new XCDatabaseStorage("test", context);

                configuration.Save(storage);

                var loadedConfiguration = XCRoot.Load(storage);

                Assert.Equal(configuration.GetHash(), loadedConfiguration.GetHash());
            }
            
        }


        [Fact]
        public void FileSystemStorage()
        {
            var configuration = Factory.CreateExampleConfiguration();

            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var storage = new XCFileSystemStorage(path, "Test"))
            {

                configuration.Save(storage);

                var loadedConfiguration = XCRoot.Load(storage);

                

                Assert.Equal(configuration.GetHash(), loadedConfiguration.GetHash());
            }

            Directory.Delete(path, true);
        }


    }
}
