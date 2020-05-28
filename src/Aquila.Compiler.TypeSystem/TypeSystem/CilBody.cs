using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PCilLabel
    {
        public ILabel Label { get; }
    }

    public class PCilLocal
    {
        public ILocal Local { get; }
    }


    public class Instruction
    {
        private Instruction()
        {
        }

        private Instruction(OpCode opCode, object argument)
        {
            OpCode = opCode;
            Argument = argument;

            if (opCode.OperandType == OperandType.InlineNone)
                Delegate = e => e.Emit(opCode);
            else
                Delegate =
                    argument switch
                    {
                        int i => e => e.Emit(OpCode, i),
                        string i => e => e.Emit(OpCode, i),
                        double i => e => e.Emit(OpCode, i),
                        float i => e => e.Emit(OpCode, i),
                        byte i => e => e.Emit(OpCode, i),
                        long i => e => e.Emit(OpCode, i),
                        IPType i => e => e.Emit(OpCode, i.ToBackend()),
                        IPParameter i => e => e.Emit(OpCode, i.ToBackend()),
                        IPMethod i => e => e.Emit(OpCode, i.ToBackend()),
                        IPField i => e => e.Emit(OpCode, i.ToBackend()),
                        IPConstructor i => e => e.Emit(OpCode, i.ToBackend()),
                        PCilLabel i => e => e.Emit(OpCode, i.Label),
                        PCilLocal i => e => e.Emit(OpCode, i.Local),

                        _ => throw new Exception("Argument not supported")
                    };
        }

        public Action<IEmitter> Delegate { get; }


        public static Instruction Create(OpCode code, object argument)
        {
            if (code.OperandType != OperandType.InlineNone && argument == null)
                throw new ArgumentNullException(nameof(argument));

            return new Instruction(code, argument);
        }

        public OpCode OpCode { get; }

        public object Argument { get; }
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