    using System.Reflection;

    namespace Aquila.Tools
{
    /// <summary>
    ///     Helper class for finding the version of Entity Framework Core being used.
    /// </summary>
    public static class ProductInfo
    {
        /// <summary>
        ///     Gets the value of the <see cref="AssemblyInformationalVersionAttribute.InformationalVersion" />
        ///     for the EntityFrameworkCore assembly.
        /// </summary>
        /// <returns> The EF Core version being used. </returns>
        public static string GetVersion()
            => typeof(ProductInfo).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}