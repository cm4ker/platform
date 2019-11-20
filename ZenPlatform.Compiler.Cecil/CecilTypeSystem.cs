using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    public class CecilTypeSystem : ITypeSystem, IAssemblyResolver
    {
        private List<CecilAssembly> _asms = new List<CecilAssembly>();

        internal CustomAssemblyResolver _asmResolver;

        private Dictionary<string, CecilAssembly> _assemblyCache = new Dictionary<string, CecilAssembly>();

        private Dictionary<TypeReference, IType> _typeReferenceCache =
            new Dictionary<TypeReference, IType>();

        private Dictionary<AssemblyDefinition, CecilAssembly> _assemblyDic
            = new Dictionary<AssemblyDefinition, CecilAssembly>();

        private Dictionary<string, IType> _unresolvedTypeCache = new Dictionary<string, IType>();

        private CustomMetadataResolver _metadataResolver;
        private CecilTypeCache _typeCache;

        public void Dispose()
        {
            foreach (var asm in _asms)
                asm.Assembly.Dispose();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name) => _asmResolver.Resolve(name);
        public AssemblyDefinition Resolve(string fullName) => _asmResolver.Resolve(fullName);

//        CecilAssembly ResolveWrapped(AssemblyNameReference name)
//        {
//            if (_assemblyCache.TryGetValue(name.FullName, out var rv))
//                return rv;
//            foreach (var asm in _asms)
//                if (asm.Assembly.Name.Equals(name))
//                    return _assemblyCache[name.FullName] = asm;
//            foreach (var asm in _asms)
//                if (asm.Assembly.Name.Name == name.Name)
//                    return _assemblyCache[name.FullName] = asm;
//            throw new AssemblyResolutionException(name);
//        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) => Resolve(name);

        public CecilTypeSystem(IEnumerable<string> paths, string targetPath = null)
        {
            if (targetPath != null)
                paths = paths.Concat(new[] {targetPath});

            _asmResolver = new CustomAssemblyResolver(this);
            _metadataResolver = new CustomMetadataResolver(this);
            _typeCache = new CecilTypeCache(this);
            foreach (var path in paths.Distinct())
            {
                var isTarget = path == targetPath;
                var asm = AssemblyDefinition.ReadAssembly(path, new ReaderParameters(ReadingMode.Deferred)
                {
                    ReadWrite = isTarget,
                    InMemory = true,
                    AssemblyResolver = this,
                    MetadataResolver = _metadataResolver,
                    SymbolReaderProvider = isTarget ? new DefaultSymbolReaderProvider(true) : null,
                    ReadSymbols = isTarget
                });

                var wrapped = RegisterAssembly(asm);
                if (path == targetPath)
                {
                    TargetAssembly = wrapped;
                    TargetAssemblyDefinition = asm;
                }
            }
        }

        public IAssembly TargetAssembly { get; private set; }

        internal MetadataResolver MetadataResolver => _metadataResolver;

        public AssemblyDefinition TargetAssemblyDefinition { get; private set; }
        public IWellKnownTypes WellKnownTypes { get; }
        public IReadOnlyList<IAssembly> Assemblies => _asms.AsReadOnly();
        public IAssembly FindAssembly(string name) => RegisterAssembly(Resolve(name));

        public IType FindType(string name)
        {
            foreach (var asm in _asms)
            {
                var found = asm.FindType(name);
                if (found != null)
                    return found;
            }

            return null;
        }

        public TypeReference GetTypeReference(string name) => GetTypeReference(FindType(name));

        public IType FindType(string name, string assembly)
            => FindAssembly(assembly)?.FindType(name);

        public IType FindType(string name, IAssembly assembly)
        {
            return assembly.FindType(name);
        }


        public TypeReference GetTypeReference(IType t) => ((ITypeReference) t).Reference;
        public MethodReference GetMethodReference(IMethod t) => ((CecilMethodBase) t).Definition;

        internal CecilAssembly FindAsm(AssemblyDefinition d)
        {
            _assemblyDic.TryGetValue(d, out var asm);

            if (asm == null)
            {
                RegisterAssembly(d);
                asm = _assemblyDic[d];
            }

            return asm;
        }

        public IType Resolve(TypeReference reference)
        {
            if (!_typeReferenceCache.TryGetValue(reference, out var rv))
            {
                TypeDefinition resolved = null;
                try
                {
                    resolved = reference.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                }

                if (resolved != null)
                {
                    rv = _typeCache.Get(reference);
                }
                else
                {
                    var key = reference.FullName;
                    if (reference is GenericParameter gp)
                        key = ((TypeReference) gp.Owner).FullName + "|GenericParameter|" + key;
                    if (!_unresolvedTypeCache.TryGetValue(key, out rv))
                        _unresolvedTypeCache[key] =
                            rv = new UnresolvedCecilType(reference);
                }

                _typeReferenceCache[reference] = rv;
            }

            return rv;
        }

        public IAssembly RegisterAssembly(AssemblyDefinition asm)
        {
            var wrapped = new CecilAssembly(this, asm);
            _asms.Add(wrapped);
            _assemblyDic[asm] = wrapped;
            _asmResolver.RegisterCustom(asm.Name.Name, asm);
            
            return wrapped;
        }


        public IAssembly CreateAndRegisterAssembly(string name, Version version, ModuleKind kind)
        {
            var def = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, version), name,
                new ModuleParameters()
                {
                    AssemblyResolver = this,
                    MetadataResolver = this._metadataResolver,
                    Kind = kind
                });

            return RegisterAssembly(def);
        }

        public IMethod Resolve(MethodDefinition method, TypeReference declaringType)
        {
            return new CecilMethod(this, method, declaringType, declaringType.Module);
        }

        internal CecilType GetTypeFor(TypeReference reference) => _typeCache.Get(reference);

        public AssemblyDefinition GetAssembly(IAssembly asm)
            => ((CecilAssembly) asm).Assembly;
    }


    interface ITypeReference
    {
        TypeReference Reference { get; }
    }
}