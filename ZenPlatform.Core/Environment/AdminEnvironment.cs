﻿using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Data;
using ZenPlatform.Core.Tools;
using ZenPlatform.Core.Logging;
using System.IO;

namespace ZenPlatform.Core.Environment
{
    /// <inheritdoc cref="IAdminEnvironment"/>
    public class AdminEnvironment : IAdminEnvironment
    {
        private ILogger _logger;


        public AdminEnvironment(IAuthenticationManager authenticationManager, IInvokeService invokeService,
            ILogger<AdminEnvironment> logger)
        {
            Sessions = new RemovingList<ISession>();
            AuthenticationManager = authenticationManager;
            AuthenticationManager.RegisterProvider(new AnonymousAuthenticationProvider());
            InvokeService = invokeService;
            _logger = logger;
        }

        public string Name => "admin";

        public IList<ISession> Sessions { get; }

        public IInvokeService InvokeService { get; }

        public void Initialize(object config)
        {
            _logger.Info("Start admin environment.");
        }

        public IAuthenticationManager AuthenticationManager { get; }

        public ISession CreateSession(IUser user)
        {
            var session = new SimpleSession(this, user);
            return session;
        }
    }
}