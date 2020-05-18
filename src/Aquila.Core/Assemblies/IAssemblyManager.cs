using System.Collections.Generic;
using System.IO;
using Aquila.Compiler;
using Aquila.Configuration.Structure;
using Aquila.Core.Assemlies;
using Aquila.Core.Contracts;
using Aquila.QueryBuilder;

namespace Aquila.Core.Assemblies
{
    public interface IAssemblyManager
    {
        /// <summary>
        /// Собрать конфигурацию
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="dbType">Тип базы данных</param>
        void BuildConfiguration(IProject configuration , SqlDatabaseType dbType);

        /// <summary>
        /// Проверяет, нужно ли пересобирать конфигурацию
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>True - Необходимо пересобрать конфигурацию; False - Всё ок</returns>
        bool CheckConfiguration(IProject configuration);

        IEnumerable<AssemblyDescription> GetAssemblies(IProject conf);

        byte[] GetAssemblyBytes(AssemblyDescription description);
    }
}