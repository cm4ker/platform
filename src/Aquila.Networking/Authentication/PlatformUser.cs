using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    /// <summary>
    /// Пользователь в системе.
    /// Обязателен для работы, потому что все манипуляции с данными будут всегда выполняться в контексте данного пользователя
    /// </summary>
    public class User : IUser
    {
        public User()
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
}