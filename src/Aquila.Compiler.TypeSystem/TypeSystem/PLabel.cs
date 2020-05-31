using System.Reflection.Emit;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PLabel
    {
        public PInstruction Instruction { get; } = PInstruction.Create(OpCodes.Nop);
    }
}