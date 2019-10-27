using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace ZenPlatform.Core.Authentication
{
    /// <summary>
    /// Пользователь в системе.
    /// Обязателен для работы, потому что все манипуляции с данными будут всегда выполняться в контексте данного пользователя
    /// </summary>
    public class PlatformUser : IPlatformUser
    {
        public PlatformUser()
        {
            Roles = new List<IRole>();
        }


        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Роли пользователя
        /// </summary>


        public List<IRole> Roles { get; }


        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Пользователь сервера. Позволяет производить процесс администрирования сервера
    /// </summary>
    public class ServerUser : IUser
    {
        /// <inheritdoc cref="Name"/>
        public string Name { get; }

        public string PasswordHash { get; set; }
    }
}