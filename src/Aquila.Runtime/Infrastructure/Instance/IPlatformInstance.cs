using System.IO;
using System.Reflection;
using Aquila.Data;
using Aquila.Runtime;

namespace Aquila.Core.Contracts.Instance
{
    public interface IPlatformInstance : IInitializableInstance<IStartupConfig>
    {
        /// <summary>
        /// Manager of data connection contexts
        /// </summary>
        DataContextManager DataContextManager { get; }

        /// <summary>
        /// Current state of database 
        /// </summary>
        DatabaseRuntimeContext DatabaseRuntimeContext { get; }

        public Assembly BLAssembly { get; }

        void UpdateAssembly(Assembly asm);

        void Deploy(Stream packageStream);

        bool PendingChanges { get; }

        void Migrate();
    }
}