using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using MethodBody = Mono.Cecil.Cil.MethodBody;
using SreOpCode = System.Reflection.Emit.OpCode;
using SreOpCodes = System.Reflection.Emit.OpCodes;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilEmitter : IEmitter
    {
        TypeReference Import(TypeReference r)
        {
            var rv = M.ImportReference(r);
            if (r is GenericInstanceType gi)
            {
                for (var c = 0; c < gi.GenericArguments.Count; c++)
                    gi.GenericArguments[c] = Import(gi.GenericArguments[c]);
            }

            return rv;
        }

        MethodReference Import(IMethod method)
        {
            return Import((CecilMethodBase) method);
        }

        MethodReference Import(CecilMethodBase method)
        {
            var def = method.Definition;
            var r = method.Reference;

            r.Name = def.Name;
            r.HasThis = def.HasThis;
            r.ExplicitThis = def.ExplicitThis;
            r.CallingConvention = def.CallingConvention;

            r.ReturnType = Import(r.ReturnType);

            return M.ImportReference(r);
        }


        MethodReference Import(IConstructor method)
        {
            return Import((CecilMethodBase) method);
        }


        ParameterDefinition Import(IParameter p)
        {
            var cecilParam = (CecilParameter) p;
            cecilParam.ParameterDefinition.ParameterType = Import(cecilParam.ParameterDefinition.ParameterType);
            return cecilParam.ParameterDefinition;
        }

        FieldReference Import(FieldReference f)
        {
            var rv = M.ImportReference(f);
            rv.FieldType = Import(rv.FieldType);
            return rv;
        }

        private List<CecilLabel> _markedLabels = new List<CecilLabel>();

        private Dictionary<SreOpCode, OpCode> Dic = SreMapper.OpCodeDic;

        private readonly MethodBody _body;
        private readonly MethodDefinition _method;
        private CecilDebugPoint _pendingDebugPoint;
        private CecilDebugPoint _lastDebugPoint;

        public CecilEmitter(ITypeSystem typeSystem, MethodDefinition method)
        {
            _method = method;
            _body = method.Body;
            TypeSystem = typeSystem;
            SymbolTable = new SymbolTable();
            _exceptionStack = new Stack<CecilTryHandler>();
        }

        public CecilEmitter(ITypeSystem typeSystem, MethodDefinition method, SymbolTable parentSymbols)
        {
            _method = method;
            _body = method.Body;
            TypeSystem = typeSystem;
            SymbolTable = new SymbolTable(parentSymbols);
        }


        public ITypeSystem TypeSystem { get; }

        IEmitter Emit(Instruction i)
        {
            _body.Instructions.Add(i);
            if (_pendingDebugPoint != null)
            {
                _pendingDebugPoint.Create(i);
                _pendingDebugPoint = null;
            }

            foreach (var ml in _markedLabels)
            {
                var next = ml.Instruction.Next;

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

        ParameterDefinition GetParameter(int arg)
        {
            if (_body.Method.HasThis)
            {
                if (arg == 0)
                    return _body.ThisParameter;
                arg--;
            }

            return _body.Method.Parameters[arg];
        }

        private Instruction CreateI(OpCode code, int arg)
        {
            if (code.OperandType == OperandType.ShortInlineArg || code.OperandType == OperandType.InlineArg)
                return Instruction.Create(code, GetParameter(arg));
            if (code.OperandType == OperandType.InlineVar || code.OperandType == OperandType.ShortInlineVar)
                return Instruction.Create(code, _body.Variables[arg]);
            return Instruction.Create(code, arg);
        }

        private ModuleDefinition M => _body.Method.Module;

        public IEmitter Emit(SreOpCode code)
            => Emit(Instruction.Create(Dic[code]));

        public IEmitter Emit(SreOpCode code, IField field)
            => Emit(Instruction.Create(Dic[code], Import(((CecilField) field).Field)));

        public IEmitter Emit(SreOpCode code, IMethod method)
            => Emit(Instruction.Create(Dic[code], M.ImportReference(((CecilMethodBase) method).Reference)));

        public IEmitter Emit(SreOpCode code, IConstructor ctor)
        {
            return Emit(Instruction.Create(Dic[code], Import(ctor)));
        }

        public IEmitter Emit(SreOpCode code, string arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, int arg)
            => Emit(CreateI(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, long arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, IType type)
            => Emit(Instruction.Create(Dic[code], Import(((ITypeReference) type).Reference)));

        public IEmitter Emit(SreOpCode code, float arg)
            => Emit(Instruction.Create(Dic[code], arg));

        public IEmitter Emit(SreOpCode code, double arg)
            => Emit(Instruction.Create(Dic[code], arg));


        class CecilLocal : ILocal
        {
            private readonly CecilTypeSystem _ts;

            public CecilLocal(CecilTypeSystem ts, VariableDefinition vd)
            {
                _ts = ts;
                Variable = vd;
            }

            public VariableDefinition Variable { get; }
            public int Index => Variable.Index;
            public IType Type => _ts.Resolve(Variable.VariableType);
        }

        class CecilLabel : ILabel
        {
            public Instruction Instruction { get; set; } = Instruction.Create(OpCodes.Nop);
        }

        public ILocal DefineLocal(IType type)
        {
            var r = Import(((ITypeReference) type).Reference);
            var def = new VariableDefinition(r);
            _body.Variables.Add(def);
            return new CecilLocal((CecilTypeSystem) TypeSystem, def);
        }

        public ILabel DefineLabel() => new CecilLabel();

        private Stack<CecilTryHandler> _exceptionStack;

        class CecilTryHandler
        {
            private readonly CecilEmitter _emitter;
            private ExceptionHandler _handler;
            private ExceptionHandlerType _type;

            private TypeReference _catchType;

            private CecilLabel _tryStart;
            private CecilLabel _tryEnd;

            private CecilLabel _handlerStart;
            private CecilLabel _handlerEnd;

            public CecilTryHandler(CecilEmitter emitter)
            {
                _emitter = emitter;
                _tryStart = (CecilLabel) _emitter.DefineLabel();
                _handlerEnd = (CecilLabel) _emitter.DefineLabel();
                _emitter.MarkLabel(_tryStart);
            }

            public void WithCatch(TypeReference type)
            {
                _tryEnd = (CecilLabel) _emitter.DefineLabel();
                _emitter.Leave(_handlerEnd);
                _emitter.MarkLabel(_tryEnd);

                //_handlerStart = (CecilLabel) _emitter.DefineLabel();
                //_emitter.MarkLabel(_handlerStart);
                _type = ExceptionHandlerType.Catch;
                _catchType = _emitter.Import(type);
            }

            public ILabel Start => _tryStart;

            public void DefferedCreate()
            {
                _emitter.Leave(_handlerEnd);
                _emitter.MarkLabel(_handlerEnd);

                _handler = new ExceptionHandler(_type);

                _handler.TryStart = _tryStart.Instruction;
                _handler.TryEnd = _tryEnd.Instruction;

                _handler.HandlerStart = _tryEnd.Instruction;
                _handler.HandlerEnd = _handlerEnd.Instruction;

                _handler.CatchType = _catchType;
                _emitter._body.ExceptionHandlers.Add(_handler);
            }
        }

        public ILabel BeginExceptionBlock()
        {
            var ce = new CecilTryHandler(this);
            _exceptionStack.Push(ce);
            return ce.Start;
        }

        public IEmitter BeginCatchBlock(IType exceptionType)
        {
            var tr = ((CecilTypeSystem) TypeSystem).GetTypeReference(exceptionType);
            _exceptionStack.Peek().WithCatch(tr);
            return this;
        }

        public IEmitter EndExceptionBlock()
        {
            _exceptionStack.Pop().DefferedCreate();
            return this;
        }

        public IEmitter Throw()
        {
            throw new NotImplementedException();
        }

        public IEmitter CatchException(IType exceptionType)
        {
            return this.Emit(SreOpCodes.Throw, exceptionType);
        }

        public IEmitter MarkLabel(ILabel label)
        {
            var cl = (CecilLabel) label;
            Emit(cl.Instruction);
            _markedLabels.Add(cl);
            return this;
        }

        public IEmitter Emit(SreOpCode code, ILabel label)
            => Emit(Instruction.Create(Dic[code], ((CecilLabel) label).Instruction));

        public IEmitter Emit(SreOpCode code, ILocal local)
            => Emit(Instruction.Create(Dic[code], ((CecilLocal) local).Variable));

        public IEmitter Emit(SreOpCode code, IParameter parameter) =>
            Emit(Instruction.Create(Dic[code], Import(parameter)));


        public bool InitLocals { get; set; }

        private static readonly Guid LanguageGuid = new Guid("9a37fc74-96b5-4dbc-8b8a-c4e603735a63");
        private static readonly Guid LanguageVendorGuid = new Guid("3c631bf9-0cbe-4aab-a24a-5e417734441c");

        class CecilDebugPoint
        {
            private readonly CecilEmitter _parent;
            public DocumentHelper Document { get; }
            public int Line { get; }
            public int Position { get; }

            public CecilDebugPoint(CecilEmitter parent, DocumentHelper document, int line, int position)
            {
                _parent = parent;
                Document = document;
                Line = line;
                Position = position;
            }

            public void Create(Instruction instruction)
            {
                // Step into doesn't work for methods without sequence points in the first instruction
                if (!_parent._method.DebugInformation.HasSequencePoints
                    && _parent._body.Instructions.Count != 0)
                    instruction = _parent._body.Instructions.First();

                var dbg = _parent._method.DebugInformation;
                if (dbg.Scope == null)
                {
                    dbg.Scope = new ScopeDebugInformation(instruction, instruction)
                    {
                        End = new InstructionOffset(),
                        Import = new ImportDebugInformation()
                    };
                }

                var realLine = Line - 1;
                var endColumn = Position;
                if (realLine < Document.Lines.Count)
                {
                    var lineString = Document.Lines[realLine];
                    for (; endColumn < lineString.Length; endColumn++)
                    {
                        var ch = lineString[endColumn];
                        if (ch == ':' || char.IsDigit(ch) || char.IsLetter(ch))
                            continue;
                        break;
                    }
                }

                endColumn++;

                var sp = new SequencePoint(instruction, Document.Document)
                {
                    StartLine = Line,
                    StartColumn = Position,
                    EndLine = Line,
                    EndColumn = endColumn
                };
                dbg.SequencePoints.Add(sp);
            }
        }

        static ConditionalWeakTable<AssemblyDefinition, Dictionary<string, DocumentHelper>>
            _documents = new ConditionalWeakTable<AssemblyDefinition, Dictionary<string, DocumentHelper>>();

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

                Document = new Document(file.FilePath)
                {
                    LanguageGuid = LanguageGuid,
                    LanguageVendorGuid = LanguageVendorGuid,
                    Type = DocumentType.Text,
                    HashAlgorithm = DocumentHashAlgorithm.SHA1,
                    Hash = hash,
                };
                var r = new StreamReader(new MemoryStream(data));
                string l;
                while ((l = r.ReadLine()) != null)
                    Lines.Add(l);
            }
        }

        public void InsertSequencePoint(IFileSource file, int line, int position)
        {
            if (!_documents.TryGetValue(_method.Module.Assembly, out var documents))
                _documents.Add(_method.Module.Assembly, documents = new Dictionary<string, DocumentHelper>());

            if (!documents.TryGetValue(file.FilePath, out var doc))
            {
                documents[file.FilePath] = doc = new DocumentHelper(file);
            }

            if (_pendingDebugPoint == null)
            {
                if (_lastDebugPoint == null ||
                    !(_lastDebugPoint.Document == doc && _lastDebugPoint.Line == line &&
                      _lastDebugPoint.Position == position))
                    _pendingDebugPoint = _lastDebugPoint = new CecilDebugPoint(this, doc, line, position);
            }
        }

        public ISymbolTable SymbolTable { get; }
    }
}