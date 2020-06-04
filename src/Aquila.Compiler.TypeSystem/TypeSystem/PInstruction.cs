using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public sealed class PInstruction
    {
        /// <summary>
        /// The opcode
        /// </summary>
        public OpCode OpCode;

        /// <summary>
        /// The opcode operand
        /// </summary>
        public object Operand;

        /// <summary>
        /// Offset of the instruction in the method body
        /// </summary>
        public uint Offset;

        // /// <summary>
        // /// PDB sequence point or <c>null</c> if none
        // /// </summary>
        // public SequencePoint SequencePoint;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PInstruction()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="opCode">Opcode</param>
        public PInstruction(OpCode opCode) => OpCode = opCode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="opCode">Opcode</param>
        /// <param name="operand">The operand</param>
        public PInstruction(OpCode opCode, object operand)
        {
            OpCode = opCode;
            Operand = operand;
        }

        /// <summary>
        /// Creates a new instruction with no operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode)
        {
            if (opCode.OperandType != OperandType.InlineNone)
                throw new ArgumentException("Must be a no-operand opcode", nameof(opCode));
            return new PInstruction(opCode);
        }

        /// <summary>
        /// Creates a new instruction with a <see cref="byte"/> operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="value">The value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, byte value)
        {
            if (opCode.OperandType != OperandType.ShortInlineI)
                throw new ArgumentException("Opcode does not have a byte operand", nameof(opCode));
            return new PInstruction(opCode, value);
        }

        /// <summary>
        /// Creates a new instruction with a <see cref="sbyte"/> operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="value">The value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, sbyte value)
        {
            if (opCode.OperandType != OperandType.ShortInlineI)
                throw new ArgumentException("Opcode does not have a sbyte operand", nameof(opCode));
            return new PInstruction(opCode, value);
        }

        /// <summary>
        /// Creates a new instruction with an <see cref="int"/> operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="value">The value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, int value)
        {
            if (opCode.OperandType != OperandType.InlineI)
                throw new ArgumentException("Opcode does not have an int32 operand", nameof(opCode));
            return new PInstruction(opCode, value);
        }

        /// <summary>
        /// Creates a new instruction with a <see cref="long"/> operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="value">The value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, long value)
        {
            if (opCode.OperandType != OperandType.InlineI8)
                throw new ArgumentException("Opcode does not have an int64 operand", nameof(opCode));
            return new PInstruction(opCode, value);
        }

        /// <summary>
        /// Creates a new instruction with a <see cref="float"/> operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="value">The value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, float value)
        {
            if (opCode.OperandType != OperandType.ShortInlineR)
                throw new ArgumentException("Opcode does not have a real4 operand", nameof(opCode));
            return new PInstruction(opCode, value);
        }

        /// <summary>
        /// Creates a new instruction with a <see cref="double"/> operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="value">The value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, double value)
        {
            if (opCode.OperandType != OperandType.InlineR)
                throw new ArgumentException("Opcode does not have a real8 operand", nameof(opCode));
            return new PInstruction(opCode, value);
        }

        /// <summary>
        /// Creates a new instruction with a string operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="s">The string</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, string s)
        {
            if (opCode.OperandType != OperandType.InlineString)
                throw new ArgumentException("Opcode does not have a string operand", nameof(opCode));
            return new PInstruction(opCode, s);
        }

        /// <summary>
        /// Creates a new instruction with an instruction target operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="target">Target instruction</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, PInstruction target)
        {
            if (opCode.OperandType != OperandType.ShortInlineBrTarget &&
                opCode.OperandType != OperandType.InlineBrTarget)
                throw new ArgumentException("Opcode does not have an instruction operand", nameof(opCode));
            return new PInstruction(opCode, target);
        }

        /// <summary>
        /// Creates a new instruction with an instruction target list operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="targets">The targets</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, IList<PInstruction> targets)
        {
            if (opCode.OperandType != OperandType.InlineSwitch)
                throw new ArgumentException("Opcode does not have a targets array operand", nameof(opCode));
            return new PInstruction(opCode, targets);
        }

        /// <summary>
        /// Creates a new instruction with a type operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="type">The type</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, PType type)
        {
            if (opCode.OperandType != OperandType.InlineType && opCode.OperandType != OperandType.InlineTok)
                throw new ArgumentException("Opcode does not have a type operand", nameof(opCode));
            return new PInstruction(opCode, type);
        }

        /// <summary>
        /// Creates a new instruction with a method/field operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="mr">The method/field</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, PMember mr)
        {
            if (opCode.OperandType != OperandType.InlineField
                && opCode.OperandType != OperandType.InlineMethod
                && opCode.OperandType != OperandType.InlineTok)
                throw new ArgumentException("Opcode does not have a field operand", nameof(opCode));
            return new PInstruction(opCode, mr);
        }

        /// <summary>
        /// Creates a new instruction with a field operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="field">The field</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, IField field)
        {
            if (opCode.OperandType != OperandType.InlineField && opCode.OperandType != OperandType.InlineTok)
                throw new ArgumentException("Opcode does not have a field operand", nameof(opCode));
            return new PInstruction(opCode, field);
        }

        /// <summary>
        /// Creates a new instruction with a method operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="method">The method</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, IMethod method)
        {
            if (opCode.OperandType != OperandType.InlineMethod && opCode.OperandType != OperandType.InlineTok)
                throw new ArgumentException("Opcode does not have a method operand", nameof(opCode));
            return new PInstruction(opCode, method);
        }

        // /// <summary>
        // /// Creates a new instruction with a token operand
        // /// </summary>
        // /// <param name="opCode">The opcode</param>
        // /// <param name="token">The token</param>
        // /// <returns>A new <see cref="Instruction"/> instance</returns>
        // public static Instruction Create(OpCode opCode, ITokenOperand token)
        // {
        //     if (opCode.OperandType != OperandType.InlineTok)
        //         throw new ArgumentException("Opcode does not have a token operand", nameof(opCode));
        //     return new Instruction(opCode, token);
        // }

        /// <summary>
        /// Creates a new instruction with a method signature operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="methodSig">The method signature</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, PMethod methodSig)
        {
            return new PInstruction(opCode, methodSig);
        }

        /// <summary>
        /// Creates a new instruction with a method parameter operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="parameter">The method parameter</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, PParameter parameter)
        {
            return new PInstruction(opCode, parameter);
        }

        /// <summary>
        /// Creates a new instruction with a method local operand
        /// </summary>
        /// <param name="opCode">The opcode</param>
        /// <param name="local">The method local</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction Create(OpCode opCode, PLocal local)
        {
            return new PInstruction(opCode, local);
        }

        public static PInstruction Create(OpCode opCode, PLabel label)
        {
            return new PInstruction(opCode, label);
        }

        /// <summary>
        /// Creates a <c>ldci4</c> instruction
        /// </summary>
        /// <param name="value">Operand value</param>
        /// <returns>A new <see cref="PInstruction"/> instance</returns>
        public static PInstruction CreateLdcI4(int value)
        {
            switch (value)
            {
                case -1: return new PInstruction(OpCodes.Ldc_I4_M1);
                case 0: return new PInstruction(OpCodes.Ldc_I4_0);
                case 1: return new PInstruction(OpCodes.Ldc_I4_1);
                case 2: return new PInstruction(OpCodes.Ldc_I4_2);
                case 3: return new PInstruction(OpCodes.Ldc_I4_3);
                case 4: return new PInstruction(OpCodes.Ldc_I4_4);
                case 5: return new PInstruction(OpCodes.Ldc_I4_5);
                case 6: return new PInstruction(OpCodes.Ldc_I4_6);
                case 7: return new PInstruction(OpCodes.Ldc_I4_7);
                case 8: return new PInstruction(OpCodes.Ldc_I4_8);
            }

            if (sbyte.MinValue <= value && value <= sbyte.MaxValue)
                return new PInstruction(OpCodes.Ldc_I4_S, (sbyte) value);
            return new PInstruction(OpCodes.Ldc_I4, value);
        }

        /// <summary>
        /// Gets the size in bytes of the instruction
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            var opCode = OpCode;
            switch (opCode.OperandType)
            {
                case OperandType.InlineBrTarget:
                case OperandType.InlineField:
                case OperandType.InlineI:
                case OperandType.InlineMethod:
                case OperandType.InlineSig:
                case OperandType.InlineString:
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.ShortInlineR:
                    return opCode.Size + 4;

                case OperandType.InlineI8:
                case OperandType.InlineR:
                    return opCode.Size + 8;

                case OperandType.InlineNone:
                case OperandType.InlinePhi:
                default:
                    return opCode.Size;

                case OperandType.InlineSwitch:
                    var targets = Operand as IList<PInstruction>;
                    return opCode.Size + 4 + (targets is null ? 0 : targets.Count * 4);

                case OperandType.InlineVar:
                    return opCode.Size + 2;

                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    return opCode.Size + 1;
            }
        }

        /// <summary>
        /// Checks whether it's one of the <c>leave</c> instructions
        /// </summary>
        public bool IsLeave() => OpCode == OpCodes.Leave || OpCode == OpCodes.Leave_S;

        /// <summary>
        /// Checks whether it's one of the <c>br</c> instructions
        /// </summary>
        public bool IsBr() => OpCode == OpCodes.Br || OpCode == OpCodes.Br_S;

        /// <summary>
        /// Checks whether it's one of the <c>brfalse</c> instructions
        /// </summary>
        public bool IsBrfalse() => OpCode == OpCodes.Brfalse || OpCode == OpCodes.Brfalse_S;

        /// <summary>
        /// Checks whether it's one of the <c>brtrue</c> instructions
        /// </summary>
        public bool IsBrtrue() => OpCode == OpCodes.Brtrue || OpCode == OpCodes.Brtrue_S;


        /// <summary>
        /// Clone this instance. The <see cref="Operand"/> and <see cref="SequencePoint"/> fields
        /// are shared by this instance and the created instance.
        /// </summary>
        public PInstruction Clone() =>
            new PInstruction
            {
                Offset = Offset,
                OpCode = OpCode,
                Operand = Operand
            };

        /// <inheritdoc/>
        public override string ToString() => "Not implemented";
    }
}