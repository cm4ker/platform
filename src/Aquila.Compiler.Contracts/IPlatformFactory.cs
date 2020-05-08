using System;

namespace Aquila.Compiler.Contracts
{
    public interface IPlatformFactory
    {
        /// <summary>
        /// Создать сборку
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="assemblyName"></param>
        /// <param name="assemblyVersion"></param>
        /// <returns></returns>
        IAssemblyBuilder CreateAssembly(ITypeSystem ts, string assemblyName, Version assemblyVersion);

        /// <summary>
        /// Создать аттрибут
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        ICustomAttributeBuilder CreateAttribute(ITypeSystem ts, IType type, params IType[] args);
    }
}