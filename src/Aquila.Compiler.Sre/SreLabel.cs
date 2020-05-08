using System.Reflection.Emit;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Sre
{
    class SreLabel : ILabel
    {
        public Label Label { get; }

        public SreLabel(Label label)
        {
            Label = label;
        }
    }
}