using System;
using System.Collections.Generic;

namespace Aquila.Core.Authentication
{
    /// <summary>
    /// Презентация пользователя в системе на всех уровнях платформы
    /// <br /> Пользователь может быть как администратором сервера, так и пользователь конкретной базы данных
    /// </summary>
    public interface IUser
    {
        //TODO: Должен ли администратор системы наследовать идиный интерфейс с пользователем прикладного решения

        /// <summary>
        /// Имя пользователя
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Пользователь прикладного решения
    /// </summary>
    public interface IPlatformUser : IUser
    {
        Guid Id { get; }

        /// <summary>
        /// Набор ролей пользователя.
        /// Наполняются в процессе авторизации пользователя (наделения правами)
        /// </summary>
        List<IRole> Roles { get; }
    }

    public interface IRole
    {
    }
}