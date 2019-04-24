using Mono.Cecil.Cil;

namespace ZenPlatform.Compiler.Generation
{
    public class Label
    {
        public Instruction Instruction = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);
    }
}