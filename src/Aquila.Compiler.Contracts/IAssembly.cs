using System;
using System.Collections.Generic;
using System.IO;

namespace Aquila.Compiler.Contracts
{
    /// <summary>
    /// Сборка
    /// </summary>
    public interface IAssembly : IEquatable<IAssembly>, ITypeSystemProvider
    {
        /// <summary>
        /// Имя сборки
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Аттрибуты
        /// </summary>
        IReadOnlyList<ICustomAttribute> CustomAttributes { get; }

        /// <summary>
        /// Найти тип в контексте сборки
        /// </summary>
        /// <param name="fullName">Полное имя типа</param>
        /// <returns>Тип</returns>
        IType FindType(string fullName);

        /// <summary>
        /// Сохранить сборку в файл
        /// </summary>
        /// <param name="fileName"></param>
        void Write(string fileName);

        /// <summary>
        /// Сохранить сборку в поток
        /// </summary>
        /// <param name="stream"></param>
        void Write(Stream stream);
    }

    public static class AssemblyExtension
    {
        public static byte[] ToBytes(this IAssembly assembly)
        {
            using (var stream = new MemoryStream())
            {
                assembly.Write(stream);
                return stream.ToArray();
            }
        }
    }
}