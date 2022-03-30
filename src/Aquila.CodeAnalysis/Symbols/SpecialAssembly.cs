namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Anumeration of well known assemblies.
    /// </summary>
    enum SpecialAssembly
    {
        /// <summary>
        /// Regular CLR assembly.
        /// </summary>
        None,

        /// <summary>
        /// Corresponds to system runtime library.
        /// </summary>
        CorLibrary,

        /// <summary>
        /// Corresponds to our runtime library (<c>Aquila.Runtime</c>).
        /// </summary>
        AquilaCorLibrary,

        /// <summary>
        /// Corresponds to System.Common.Data
        /// </summary>
        CommonData,
        
        /// <summary>
        /// Corresponds to System.Collections.Immutable
        /// </summary>
        SystemCollectionsImmutable,
        
        /// <summary>
        /// Corresponds to System.Linq
        /// </summary>
        SystemLinq
    }
}