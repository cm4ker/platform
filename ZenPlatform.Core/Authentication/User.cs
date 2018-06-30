using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace ZenPlatform.Core.Authentication
{
    /// <summary>
    /// Пользователь в системе.
    /// Обязателен для работы, потому что все манипуляции с данными будут всегда выполняться в контексте данного пользователя
    /// </summary>
    public class User
    {
        public User()
        {
            Roles = new List<RoleBase>();
        }


        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Роли пользователя
        /// </summary>


        public List<RoleBase> Roles { get; }


        public override string ToString()
        {
            return Name;
        }
    }
}