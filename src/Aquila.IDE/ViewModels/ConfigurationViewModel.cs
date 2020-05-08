using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquila.IDE.ViewModels
{
    public class ConfigurationViewModel
    {
        public IList<ConfigurationNode> Tree { get; }
    }

    public class ConfigurationNode
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        private List<ConfigurationNode> Childrens { get; set; }
    }

    public class Fake
    {
        void CreateConfiguration()
        {
        }
    }
}