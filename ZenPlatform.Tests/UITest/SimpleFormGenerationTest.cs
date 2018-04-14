using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.Data;
using ZenPlatform.UIGeneration2;
using ZenPlatform.UIGeneration2.CodeGeneration;


namespace ZenPlatform.Tests.UITest
{
    [TestClass]
    public class SimpleFormGenerationTest
    {

      

        [TestMethod]
        public void GenerateFromClass()
        {

            var com = ConfigurationFactory.CreateDocumentComponent();

            var gfm = new GenerateFormModel(ProjectType.Wpf);
            gfm.AddNameToUIControls = true;
            gfm.CreateObjectDefinition = new CreateObjectDefinition(CreateObject.WpfEntryForm);

            var layout = new List<PropertyInformationViewModel>();
            foreach (var prop in com.Objects.First().Properties)
            {
                var propinfo = new PropertyInformationViewModel(true, prop.Name, "TextBox", "DocumentComponent", ProjectType.Wpf,
                    prop.Owner.Name, false, false);

                layout.Add(propinfo);

            }
            gfm.ColumnLayouts.Add(layout);

            var codeGeneration = new UIGeneration();
            var xaml = codeGeneration.Generate(gfm).Trim();

            Console.WriteLine(xaml);
        }
    }
}
