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
        private Dictionary<string, CecilAssembly> _assemblyCache = new Dictionary<string, CecilAssembly>();

        private Dictionary<TypeReference, IType> _typeReferenceCache =
            new Dictionary<TypeReference, IType>();

        private Dictionary<AssemblyDefinition, CecilAssembly> _assemblyDic
            = new Dictionary<AssemblyDefinition, CecilAssembly>();

        private Dictionary<string, IType> _unresolvedTypeCache = new Dictionary<string, IType>();

        private CustomMetadataResolver _resolver;
        private CecilTypeCache _typeCache;

        public void Dispose()
        {
            foreach (var asm in _asms)
                asm.Assembly.Dispose();
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name) => ResolveWrapped(name)?.Assembly;

        CecilAssembly ResolveWrapped(AssemblyNameReference name)
        {
            if (_assemblyCache.TryGetValue(name.FullName, out var rv))
                return rv;
            foreach (var asm in _asms)
                if (asm.Assembly.Name.Equals(name))
                    return _assemblyCache[name.FullName] = asm;
            foreach (var asm in _asms)
                if (asm.Assembly.Name.Name == name.Name)
                    return _assemblyCache[name.FullName] = asm;
            throw new AssemblyResolutionException(name);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) => Resolve(name);

        public CecilTypeSystem(IEnumerable<string> paths, string targetPath = null)
        {
            if (targetPath != null)
                paths = paths.Concat(new[] {targetPath});
            _resolver = new CustomMetadataResolver(this);
            _typeCache = new CecilTypeCache(this);
            foreach (var path in paths.Distinct())
            {
                var isTarget = path == targetPath;
                var asm = AssemblyDefinition.ReadAssembly(path, new ReaderParameters(ReadingMode.Deferred)
                {
                    ReadWrite = isTarget,
                    InMemory = true,
                    AssemblyResolver = this,
                    MetadataResolver = _resolver,
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
        public AssemblyDefinition TargetAssemblyDefinition { get; private set; }
        public IReadOnlyList<IAssembly> Assemblies => _asms.AsReadOnly();
        public IAssembly FindAssembly(string name) => _asms.FirstOrDefault(a => a.Assembly.Name.Name == name);

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

        public IType FindType(string name, string assembly)
            => FindAssembly(assembly)?.FindType(name);

        public IType FindType(string name, IAssembly assembly)
        {
            return assembly.FindType(name);
        }


        public TypeReference GetTypeReference(IType t) => ((ITypeReference) t).Reference;
        public MethodReference GetMethodReference(IMethod t) => ((CecilMethod) t).IlReference;

        internal CecilAssembly FindAsm(AssemblyDefinition d)
        {
            _assemblyDic.TryGetValue(d, out var asm);
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
            return wrapped;
        }

        public IAssembly CreateAndRegisterAssembly(string name, Version version, ModuleKind kind)
        {
            var def = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, version), name,
                new ModuleParameters()
                {
                    AssemblyResolver = this,
                    MetadataResolver = this._resolver,
                    Kind = kind
                });

            return RegisterAssembly(def);
        }

        public IMethod Resolve(MethodDefinition method, TypeReference declaringType)
        {
            return new CecilMethod(this, method, declaringType);
        }

        internal CecilType GetTypeFor(TypeReference reference) => _typeCache.Get(reference);


        public ITypeBuilder CreateTypeBuilder(TypeDefinition def)
        {
            return new CecilTypeBuilder(this, FindAsm(def.Module.Assembly), def);
        }

        public AssemblyDefinition GetAssembly(IAssembly asm)
            => ((CecilAssembly) asm).Assembly;
    }


    interface ITypeReference
    {
        TypeReference Reference { get; }
    }
}