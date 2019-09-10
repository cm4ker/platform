using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace ZenPlatform.Core.Assemlies
{
    public class PlatformAssemblyLoadContext: AssemblyLoadContext
    {
        private IClientAssemblyManager _clientAssemblyManager;

        public bool Updated { get; private set; }
        public PlatformAssemblyLoadContext(IClientAssemblyManager clientAssemblyManager)
        {
            _clientAssemblyManager = clientAssemblyManager;
            
        }


        

        protected override Assembly Load(AssemblyName assemblyName)
        {
            try
            {
                if (!Updated)
                {
                    _clientAssemblyManager.UpdateAssemblyes();

                    Updated = true;
                }

                return LoadFromStream(
                _clientAssemblyManager.GetAssembly(assemblyName.Name));
            } catch (Exception ex)
            {
                return null;
            }
        }
    }
}
