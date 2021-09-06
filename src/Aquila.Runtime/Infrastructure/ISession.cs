using System;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Data;
using Aquila.Runtime;

namespace Aquila.Core.Contracts
{
    public interface ISession : IRemovable
    {
        /// <summary>
        /// Session id 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The user
        /// </summary>
        IUser User { get; }


        /// <summary>
        /// Current context for database access
        /// </summary>
        DataConnectionContext DataContext { get; }

        /// <summary>
        /// Set session parameter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetSessionParameter(string key, object value);


        /// <summary>
        /// Get session parameter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        object GetSessionParameter(string key, object value);


        /// <summary>
        /// The environment
        /// </summary>
        IPlatformInstance Instance { get; }

        /// <summary>
        /// Close the session
        /// </summary>
        void Close();
    }
}