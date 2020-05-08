using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Xunit;
using Aquila.Compiler;
using Aquila.Compiler.Cecil;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Platform;
using Aquila.Configuration;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Structure;
using Aquila.Configuration.Structure.Data.Types.Complex;
using Aquila.ConfigurationExample;
using Aquila.QueryBuilder;


namespace Aquila.Tests.Conf
{
    public class SimpleFunctionsCoreTest
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
            Assert.Equal(conf.Data.ObjectTypes.Count(), loadedConf.Data.ObjectTypes.Count());
            Assert.Equal(conf.Data.ObjectTypes.First().GetProperties().Any(x => x.Unique),
                loadedConf.Data.ObjectTypes.First().GetProperties().Any(x => x.Unique));

            Assert.Equal(5,
                loadedConf.Data.ObjectTypes.FirstOrDefault(x => x.Name == "Invoice")
                    .GetPropertyByName("CompositeProperty").Types.Count);
        }


        [Fact]
        public void BuildTest()
        {
            var conf = Factory.CreateExampleConfiguration();
            IAssemblyPlatform pl = new CecilAssemblyPlatform();

            XCCompiler c = new XCCompiler();
            var asm = c.Build(conf, CompilationMode.Server, SqlDatabaseType.SqlServer);

            asm.Write("Debug.bll");
        }
    }
}