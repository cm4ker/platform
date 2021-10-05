using System;
using System.Collections.Generic;

namespace Aquila.Core.Contracts.Authentication
{
    /// <summary>
    /// Platform user
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        string Name { get; }

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