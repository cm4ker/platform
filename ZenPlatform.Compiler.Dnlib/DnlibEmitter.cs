using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using IField = ZenPlatform.Compiler.Contracts.IField;
using IMethod = ZenPlatform.Compiler.Contracts.IMethod;
using IType = ZenPlatform.Compiler.Contracts.IType;
using SreOpCode = System.Reflection.Emit.OpCode;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibEmitter : IEmitter
    {
        private readonly MethodDef _method;
        private CilBody _body;
        private Dictionary<SreOpCode, OpCode> Dic = SreMapper.OpCodeDic;
        private DnlibTypeSystem _ts;
        private List<DnlibLabel> _markedLabels = new List<DnlibLabel>();
        private DnlibDebugPoint _pendingDebugPoint;

        public DnlibEmitter(ITypeSystem typeSystem, MethodDef method)
        {
            _method = method;
            _ts = (DnlibTypeSystem) typeSystem;
            _body = _method.Body;
        }

        private dnlib.DotNet.IMethod ImportMethod(DnlibMethodBase method)
        {
            return _method.Module.Import(method.MethofRef);
        }

        private dnlib.DotNet.ITypeDefOrRef ImportType(DnlibType type)
        {
            return (ITypeDefOrRef) _method.Module.Import(type.TypeRef);
        }


        private IEmitter Emit(Instruction i)
        {
            _body.Instructions.Add(i);
            if (_pendingDebugPoint != null)
            {
                _pendingDebugPoint.Create(i);
                _pendingDebugPoint = null;
            }

            foreach (var ml in _markedLabels)
            {
                var nextIndex = _body.Instructions.IndexOf(ml.Instruction) + 1;
                var next = _body.Instructions[nextIndex];

                foreach (var instruction in _body.Instructions)
                    if (instruction.Operand == ml.Instruction)
                        instruction.Operand = next;

                foreach (var handler in _body.ExceptionHandlers)
                {
                    if (handler.HandlerStart == ml.Instruction)
                        handler.HandlerStart = next;

                    if (handler.HandlerEnd == ml.Instruction)
                        handler.HandlerEnd = next;

                    if (handler.TryStart == ml.Instruction)
                        handler.TryStart = next;

                    if (handler.TryEnd == ml.Instruction)
                        handler.TryEnd = next;
                }

                _body.Instructions.Remove(ml.Instruction);

                ml.Instruction = next;
            }

            _markedLabels.Clear();
            return this;
        }

        public ITypeSystem TypeSystem { get; }


        public IEmitter Emit(SreOpCode code) => Emit(Instruction.Create(Dic[code]));


        public IEmitter Emit(SreOpCode code, IField field)
            => Emit(Instruction.Create(Dic[code], ((DnlibField) field).FieldDef));


        public IEmitter Emit(SreOpCode code, IMethod method)
            => Emit(Instruction.Create(Dic[code], ImportMethod((DnlibMethodBase) method)));


        public IEmitter Emit(SreOpCode code, IConstructor ctor)
            => Emit(Instruction.Create(Dic[code], ((DnlibConstructor) ctor).MethodDef));

        public IEmitter Emit(SreOpCode code, string arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, int arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, long arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, IType type)
            => Emit(Instruction.Create(Dic[code], ImportType((DnlibType) type)));

        public IEmitter Emit(SreOpCode code, float arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, double arg)
            => Emit(Instruction.Create(Dic[code], arg));


        class DnlibLocal : ILocal
        {
            private readonly DnlibTypeSystem _ts;
            private readonly ITypeDefOrRef _typeRef;
            private DnlibContextResolver _v;
            public Local LocalDef { get; }

            public DnlibLocal(DnlibTypeSystem ts, Local localDef, ITypeDefOrRef typeRef)
            {
                _ts = ts;
                _typeRef = typeRef;
                LocalDef = localDef;


                _v = new DnlibContextResolver(ts, typeRef.Module);
            }


            public int Index => LocalDef.Index;
            public IType Type => _v.GetType(LocalDef.Type);
        }

        class DnlibLabel : ILabel
        {
            public Instruction Instruction { get; set; } = Instruction.Create(OpCodes.Nop);
        }

        public ILocal DefineLocal(IType type)
        {
            var t = ((DnlibType) type).TypeRef;
            var loc = new Local(t.ToTypeSig());
            _body.Variables.Add(loc);

            return new DnlibLocal(_ts, loc, t);
        }

        public ILabel DefineLabel() => new DnlibLabel();

        public ILabel BeginExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public IEmitter BeginCatchBlock(IType exceptionType)
        {
            throw new NotImplementedException();
        }

        public IEmitter CatchException(IType exceptionType)
        {
            throw new NotImplementedException();
        }

        public IEmitter EndExceptionBlock()
        {
            throw new NotImplementedException();
        }

        public IEmitter Throw()
        {
            throw new NotImplementedException();
        }

        public IEmitter MarkLabel(ILabel label)
        {
            var dl = (DnlibLabel) label;
            Emit(dl.Instruction);
            _markedLabels.Add(dl);
            return this;
        }

        public IEmitter Emit(SreOpCode code, ILabel label)
            => Emit(Instruction.Create(Dic[code], ((DnlibLabel) label).Instruction));

        public IEmitter Emit(SreOpCode code, ILocal local) =>
            Emit(Instruction.Create(Dic[code], ((DnlibLocal) local).LocalDef));

        public IEmitter Emit(SreOpCode code, IParameter parameter)
        {
            throw new NotImplementedException();
        }

        public bool InitLocals { get; set; }

        public void InsertSequencePoint(IFileSource file, int line, int position)
        {
            throw new NotImplementedException();
        }

        public ISymbolTable SymbolTable { get; }


        class DnlibDebugPoint
        {
            private readonly DnlibEmitter _parent;
            public DocumentHelper Document { get; }
            public int Line { get; }
            public int Position { get; }

            public DnlibDebugPoint(DnlibEmitter parent, DocumentHelper document, int line, int position)
            {
                _parent = parent;
                Document = document;
                Line = line;
                Position = position;
            }

            public void Create(Instruction instruction)
            {
//                // Step into doesn't work for methods without sequence points in the first instruction
//                if (!_parent._method.Debug
//                    && _parent._body.Instructions.Count != 0)
//                    instruction = _parent._body.Instructions.First();
//
//                var dbg = _parent._method.DebugInformation;
//                if (dbg.Scope == null)
//                {
//                    dbg.Scope = new ScopeDebugInformation(instruction, instruction)
//                    {
//                        End = new InstructionOffset(),
//                        Import = new ImportDebugInformation()
//                    };
//                }
//
//                var realLine = Line - 1;
//                var endColumn = Position;
//                if (realLine < Document.Lines.Count)
//                {
//                    var lineString = Document.Lines[realLine];
//                    for (; endColumn < lineString.Length; endColumn++)
//                    {
//                        var ch = lineString[endColumn];
//                        if (ch == ':' || char.IsDigit(ch) || char.IsLetter(ch))
//                            continue;
//                        break;
//                    }
//                }
//
//                endColumn++;
//
//                var sp = new SequencePoint(instruction, Document.Document)
//                {
//                    StartLine = Line,
//                    StartColumn = Position,
//                    EndLine = Line,
//                    EndColumn = endColumn
//                };
//                dbg.SequencePoints.Add(sp);
            }
        }

        class DocumentHelper
        {
            public Document Document { get; }
            public List<string> Lines { get; } = new List<string>();

            public DocumentHelper(IFileSource file)
            {
                var data = file.FileContents;
                byte[] hash;
                using (var sha1 = SHA1.Create())
                    hash = sha1.ComputeHash(data);

//                Document = new Document(file.FilePath)
//                {
//                    LanguageGuid = LanguageGuid,
//                    LanguageVendorGuid = LanguageVendorGuid,
//                    Type = DocumentType.Text,
//                    HashAlgorithm = DocumentHashAlgorithm.SHA1,
//                    Hash = hash,
//                };
                var r = new StreamReader(new MemoryStream(data));
                string l;
                while ((l = r.ReadLine()) != null)
                    Lines.Add(l);
            }
        }
    }
}