using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Common;

namespace Aquila.WebServiceComponent.Configuration.Editors
{
    public class MethodEditor
    {
        private MDMethod _mp;
        private List<ArgumentEditor> _arguments;

        public MethodEditor(MDMethod mp)
        {
            _mp = mp;
            _arguments = new List<ArgumentEditor>(mp.Arguments.Select(x => new ArgumentEditor(x)));
        }

        public string Name
        {
            get => _mp.Name;
            set => _mp.Name = value;
        }

        public ArgumentEditor CreateArgument()
        {
            var a = new MDArgument();
            var ae = new ArgumentEditor(a);
            _mp.Arguments.Add(a);

            _arguments.Add(ae);
            return ae;
        }

        public IEnumerable<ArgumentEditor> ArgumentEditors => _arguments;

        public MethodEditor SetReturnType(MDType type)
        {
            _mp.ResultType = type;
            return this;
        }

        public MDType ReturnType => _mp.ResultType;
    }
}