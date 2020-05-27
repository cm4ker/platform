using System.Collections.Generic;
using System.Reflection.Emit;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class Instruction
    {
        public Instruction()
        {
        }

        public OpCode OpCode { get; set; }

        public object Argument { get; set; }
    }

    public class CilBody
    {
        public CilBody()
        {
            Instructions = new List<Instruction>();
        }

        public List<Instruction> Instructions { get; }
    }
}