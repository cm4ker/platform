using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PLabel
    {
        public PLabel()
        {
            Instruction = PInstruction.Create(OpCodes.Nop, this);
        }

        public PInstruction Instruction { get; }

        public ILabel BackendLabel { get; set; }
    }
}