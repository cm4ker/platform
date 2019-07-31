using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Xunit;
using ZenPlatform.Compiler.Cecil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Tests.Common;


namespace ZenPlatform.Tests.Conf
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
            Assert.Equal(conf.Data.ComponentTypes.Count(), loadedConf.Data.ComponentTypes.Count());
            Assert.Equal(conf.Data.ComponentTypes.First().GetProperties().Any(x => x.Unique),
                loadedConf.Data.ComponentTypes.First().GetProperties().Any(x => x.Unique));

            Assert.Equal(5,
                loadedConf.Data.ComponentTypes.FirstOrDefault(x => x.Name == "Invoice")
                    .GetPropertyByName("CompositeProperty").Types.Count);
        }


        [Fact]
        public void BuildTest()
        {
            var conf = Factory.CreateExampleConfiguration();
            IAssemblyPlatform pl = new CecilAssemblyPlatform();
            var asm = pl.CreateAssembly("Debug");

            //STAGE0
            foreach (var t in conf.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage0(t, asm);
            }

            var list = new Dictionary<XCObjectTypeBase, IType>();

            //STAGE1
            foreach (var t in conf.Data.ComponentTypes)
            {
                var b = t.Parent.ComponentImpl.Generator.Stage1(t, asm);
                list.Add(t, b);
            }

            //STAGE2
            foreach (var t in conf.Data.ComponentTypes)
            {
                t.Parent.ComponentImpl.Generator.Stage2(t, (ITypeBuilder) list[t], list.ToImmutableDictionary(), asm);
            }

            asm.Write("Debug.bll");
        }
    }
}