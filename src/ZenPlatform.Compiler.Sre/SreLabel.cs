using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
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