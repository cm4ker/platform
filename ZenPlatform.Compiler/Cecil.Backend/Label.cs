using Mono.Cecil.Cil;

namespace ZenPlatform.Compiler.Cecil.Backend
{
    public class Label
    {
        public Instruction Instruction = Instruction.Create(OpCodes.Nop);

        public Label()
        {
        }

        public Label(Instruction i)
        {
            Instruction = i;
        }
    }
}