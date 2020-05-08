﻿using System.Collections.Generic;
using System.IO;
using ZenPlatform.Compiler;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Assemlies;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.Core.Assemblies
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