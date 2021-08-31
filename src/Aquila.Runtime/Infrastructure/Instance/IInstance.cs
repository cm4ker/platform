using System.Collections.Generic;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Network;

namespace Aquila.Core.Contracts.Instance
{
    public interface IInstance
    {
        /// <summary>
        /// Instance name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///  Sessions on this instance
        /// </summary>
        IList<ISession> Sessions { get; }


        /// <summary>
        /// Service RPC
        /// </summary>
        IInvokeService InvokeService { get; }

        /// <summary>
        /// Auth manager
        /// </summary>
        IAuthenticationManager AuthenticationManager { get; }

        /// <summary>
        /// Create session
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Session instance</returns>
        ISession CreateSession(IUser user);
    }
}