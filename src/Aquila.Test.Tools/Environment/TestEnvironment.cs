using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Aquila.Core.Authentication;
using Aquila.Core.Sessions;
using Aquila.Data;
using Aquila.Core.Tools;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Authentication;
using Aquila.Core.Contracts.Instance;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Instance;
using Aquila.Logging;


namespace Aquila.Core.Test.Environment
{
    // public class TestAsmManager : IAssemblyManager
    // {
    //     public void BuildConfiguration(IProject configuration, SqlDatabaseType dbType)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public bool CheckConfiguration(IProject configuration)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public IEnumerable<AssemblyDescription> GetAssemblies(IProject conf)
    //     {
    //         yield return new AssemblyDescription()
    //         {
    //             AssemblyHash = "Some Hash", ConfigurationHash = "Some Hash", Name = "Server", Type = AssemblyType.Server
    //         };
    //     }
    //
    //     public byte[] GetAssemblyBytes(AssemblyDescription description)
    //     {
    //         return File.ReadAllBytes(typeof(EntryPoint).Assembly.Location);
    //     }
    // }


    // public class TestInstance : IPlatformInstance
    // {
    //     private IStartupConfig _config;
    //
    //     private ILogger _logger;
    //     //private IAssemblyManager _assemblyManager;
    //
    //     public IList<ISession> Sessions { get; }
    //
    //     public IInvokeService InvokeService { get; }
    //
    //     public IAuthenticationManager AuthenticationManager { get; }
    //
    //     public string Name => "Library";
    //
    //     public DataContextManager DataContextManager => throw new NotImplementedException();
    //
    //     // public IProject Configuration => ConfigurationFactory.Create();
    //
    //     public TestInstance(IAuthenticationManager authenticationManager, IInvokeService invokeService,
    //         ILogger<TestInstance> logger
    //     )
    //     {
    //         Sessions = new RemovingList<ISession>();
    //         AuthenticationManager = authenticationManager;
    //         AuthenticationManager.RegisterProvider(new AnonymousAuthenticationProvider());
    //         InvokeService = invokeService;
    //
    //
    //         _logger = logger;
    //     }
    //
    //     public ISession CreateSession(IUser user)
    //     {
    //         var session = new SimpleSession(this, user);
    //         return session;
    //     }
    //
    //     public void Initialize(IStartupConfig config)
    //     {
    //     }
    //
    //     public ILinkFactory LinkFactory { get; }
    // }
}