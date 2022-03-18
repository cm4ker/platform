using System.Collections.Generic;
using Aquila.Core.Contracts.Authentication;

namespace Aquila.Core.Authentication
{
    public abstract class RoleBase
    {
        protected RoleBase()
        {
            Rights = new List<RightBase>();
        }

        public string Name { get; set; }

        /// <summary>
        /// Права роли
        /// </summary>
        public List<RightBase> Rights { get; }
    }

    /// <summary>
    /// Базовый класс прав
    /// </summary>
    public abstract class RightBase
    {
    }
}