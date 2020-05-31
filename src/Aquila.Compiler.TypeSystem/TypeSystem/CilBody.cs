using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;
using Avalonia.Controls.Templates;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class CilBody
    {
        public Guid Id { get; }

        public List<PInstruction> Labels { get; } = new List<PInstruction>();

        public List<PLocal> Locals { get; } = new List<PLocal>();

        public CilBody()
        {
            Instructions = new List<PInstruction>();
        }

        public List<PInstruction> Instructions { get; }

        public CilBody Emit(OpCode code)
        {
            Instructions.Add(PInstruction.Create(code));
            return this;
        }

        public CilBody Emit(OpCode code, IPField field)
        {
            Instructions.Add(PInstruction.Create(code, field));
            return this;
        }

        public CilBody Emit(OpCode code, IPMethod method)
        {
            Instructions.Add(PInstruction.Create(code, method));
            return this;
        }

        public CilBody Emit(OpCode code, IPConstructor ctor)
        {
            Instructions.Add(PInstruction.Create(code, ctor));
            return this;
        }

        public CilBody Emit(OpCode code, string arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public CilBody Emit(OpCode code, int arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public CilBody Emit(OpCode code, long arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public CilBody Emit(OpCode code, IPType type)
        {
            Instructions.Add(PInstruction.Create(code, type));
            return this;
        }

        public CilBody Emit(OpCode code, float arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public CilBody Emit(OpCode code, double arg)
        {
            Instructions.Add(PInstruction.Create(code, arg));
            return this;
        }

        public PLocal DefineLocal(IPType type)
        {
            var local = new PLocal(type, 0);

            Locals.Add(local);

            return local;
        }

        public PLabel DefineLabel()
        {
            return new PLabel();
        }

        public CilBody MarkLabel(PLabel label)
        {
            Instructions.Add(label.Instruction);
            return this;
        }

        public CilBody Emit(OpCode code, PLabel label)
        {
            Instructions.Add(PInstruction.Create(code, label.Instruction));
            return this;
        }

        public CilBody Emit(OpCode code, PLocal local)
        {
            Instructions.Add(PInstruction.Create(code, local));
            return this;
        }

        public CilBody Emit(OpCode code, IPParameter parameter)
        {
            Instructions.Add(PInstruction.Create(code, parameter));
            return this;
        }
    }
}