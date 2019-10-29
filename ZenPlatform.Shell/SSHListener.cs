﻿using System;
using System.ComponentModel.Design;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Core.Assemblies;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Core.CacheService;
using ZenPlatform.Core.ClientServices;
using ZenPlatform.Core.Configuration;
using ZenPlatform.Core.Environment;
using ZenPlatform.Core.Logging;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Serialisers;
using ZenPlatform.Core.Settings;
using ZenPlatform.Core.Tools;
using ZenPlatform.Data;
using ZenPlatform.Shell.Terminal;
using ZenPlatform.SSH;
using ZenPlatform.SSH.Services;
using AuthenticationManager = ZenPlatform.Core.Authentication.AuthenticationManager;
using Channel = ZenPlatform.Core.Network.Channel;

namespace ZenPlatform.Shell
{
    public class SSHListener : INetworkListener
    {
        private SshServer _server;

        private int _windowWidth, _windowHeight;
        private IServiceProvider _provider;
        private ServerConnectionFactory _cFactory;

        public SSHListener(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Start(IPEndPoint endPoint, ServerConnectionFactory connectionFactory)
        {
            _cFactory = connectionFactory;
            _server = new SshServer(new StartingInfo(endPoint.Address, endPoint.Port));

            _server.AddHostKey("ssh-rsa",
                "BwIAAACkAABSU0EyAAQAAAEAAQADKjiW5UyIad8ITutLjcdtejF4wPA1dk1JFHesDMEhU9pGUUs+HPTmSn67ar3UvVj/1t/+YK01FzMtgq4GHKzQHHl2+N+onWK4qbIAMgC6vIcs8u3d38f3NFUfX+lMnngeyxzbYITtDeVVXcLnFd7NgaOcouQyGzYrHBPbyEivswsnqcnF4JpUTln29E1mqt0a49GL8kZtDfNrdRSt/opeexhCuzSjLPuwzTPc6fKgMc6q4MBDBk53vrFY2LtGALrpg3tuydh3RbMLcrVyTNT+7st37goubQ2xWGgkLvo+TZqu3yutxr1oLSaPMSmf9bTACMi5QDicB3CaWNe9eU73MzhXaFLpNpBpLfIuhUaZ3COlMazs7H9LCJMXEL95V6ydnATf7tyO0O+jQp7hgYJdRLR3kNAKT0HU8enE9ZbQEXG88hSCbpf1PvFUytb1QBcotDy6bQ6vTtEAZV+XwnUGwFRexERWuu9XD6eVkYjA4Y3PGtSXbsvhwgH0mTlBOuH4soy8MV4dxGkxM8fIMM0NISTYrPvCeyozSq+NDkekXztFau7zdVEYmhCqIjeMNmRGuiEo8ppJYj4CvR1hc8xScUIw7N4OnLISeAdptm97ADxZqWWFZHno7j7rbNsq5ysdx08OtplghFPx4vNHlS09LwdStumtUel5oIEVMYv+yWBYSPPZBcVY5YFyZFJzd0AOkVtUbEbLuzRs5AtKZG01Ip/8+pZQvJvdbBMLT1BUvHTrccuRbY03SHIaUM3cTUc=");
            _server.AddHostKey("ssh-dss",
                "BwIAAAAiAABEU1MyAAQAAG+6KQWB+crih2Ivb6CZsMe/7NHLimiTl0ap97KyBoBOs1amqXB8IRwI2h9A10R/v0BHmdyjwe0c0lPsegqDuBUfD2VmsDgrZ/i78t7EJ6Sb6m2lVQfTT0w7FYgVk3J1Deygh7UcbIbDoQ+refeRNM7CjSKtdR+/zIwO3Qub2qH+p6iol2iAlh0LP+cw+XlH0LW5YKPqOXOLgMIiO+48HZjvV67pn5LDubxru3ZQLvjOcDY0pqi5g7AJ3wkLq5dezzDOOun72E42uUHTXOzo+Ct6OZXFP53ZzOfjNw0SiL66353c9igBiRMTGn2gZ+au0jMeIaSsQNjQmWD+Lnri39n0gSCXurDaPkec+uaufGSG9tWgGnBdJhUDqwab8P/Ipvo5lS5p6PlzAQAAACqx1Nid0Ea0YAuYPhg+YolsJ/ce");
            _server.ConnectionAccepted += server_ConnectionAccepted;

            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }


        void server_ConnectionAccepted(object sender, Session e)
        {
            Console.WriteLine("Accepted a client.");

            e.ServiceRegistered += e_ServiceRegistered;
            e.KeysExchanged += e_KeysExchanged;
        }

        private void e_KeysExchanged(object sender, KeyExchangeArgs e)
        {
            foreach (var keyExchangeAlg in e.KeyExchangeAlgorithms)
            {
                Console.WriteLine("Key exchange algorithm: {0}", keyExchangeAlg);
            }
        }

        void e_ServiceRegistered(object sender, SshService e)
        {
            var session = (Session) sender;
            Console.WriteLine("Session {0} requesting {1}.",
                BitConverter.ToString(session.SessionId).Replace("-", ""), e.GetType().Name);

            if (e is UserauthService)
            {
                var service = (UserauthService) e;
                service.Userauth += service_Userauth;
            }
            else if (e is ConnectionService)
            {
                var service = (ConnectionService) e;
                service.CommandOpened += service_CommandOpened;
                service.EnvReceived += service_EnvReceived;
                service.PtyReceived += service_PtyReceived;
                service.TcpForwardRequest += service_TcpForwardRequest;
            }
        }

        static void service_TcpForwardRequest(object sender, TcpRequestArgs e)
        {
            Console.WriteLine("Received a request to forward data to {0}:{1}", e.Host, e.Port);

            var allow = true; // func(e.Host, e.Port, e.AttachedUserauthArgs);

            if (!allow)
                return;

            var tcp = new TcpForwardService(e.Host, e.Port, e.OriginatorIP, e.OriginatorPort);
            e.Channel.DataReceived += (ss, ee) => tcp.OnData(ee);
            e.Channel.CloseReceived += (ss, ee) => tcp.OnClose();
            tcp.DataReceived += (ss, ee) => e.Channel.SendData(ee);
            tcp.CloseReceived += (ss, ee) => e.Channel.SendClose();
            tcp.Start();
        }

        void service_PtyReceived(object sender, PtyArgs e)
        {
            Console.WriteLine("Request to create a PTY received for terminal type {0}", e.Terminal);
            _windowWidth = (int) e.WidthChars;
            _windowHeight = (int) e.HeightRows;
        }

        void service_EnvReceived(object sender, EnvironmentArgs e)
        {
            Console.WriteLine("Received environment variable {0}:{1}", e.Name, e.Value);
        }

        void service_Userauth(object sender, UserauthArgs e)
        {
            Console.WriteLine("Client {0} fingerprint: {1}.", e.KeyAlgorithm, e.Fingerprint);

            //Auth provider here
            e.Result = true;
        }

        void service_CommandOpened(object sender, CommandRequestedArgs e)
        {
            Console.WriteLine($"Channel {e.Channel.ServerChannelId} runs {e.ShellType}: \"{e.CommandText}\".");

            var allow = true; // func(e.ShellType, e.CommandText, e.AttachedUserauthArgs);

            if (!allow)
                return;

            if (e.ShellType == "shell")
            {
                var sshTransportServer = new SSHTransportServer(e.Channel);

                var connection = _cFactory.CreateConnection(sshTransportServer);

                connection.Open();

//                //ITerminal terminal = new Terminal("cmd.exe", windowWidth, windowHeight);
//                ITerminalSession terminal = new TerminalSession(new TerminalSize(_windowWidth, _windowHeight));
//
//                e.Channel.DataReceived += (ss, ee) => terminal.ConsumeData(ee);
//                e.Channel.CloseReceived += (ss, ee) => terminal.Close();
//                e.Channel.SizeChanged += (ss, ee) => terminal.ChangeSize(ee);
//
//                terminal.DataReceived += (ss, ee) => e.Channel.SendData(ee);
//                terminal.CloseReceived += (ss, ee) => e.Channel.SendClose(ee);
//
//                terminal.Run();
            }
            else if (e.ShellType == "exec")
            {
                var parser =
                    new Regex(@"(?<cmd>git-receive-pack|git-upload-pack|git-upload-archive) \'/?(?<proj>.+)\.git\'");
                var match = parser.Match(e.CommandText);
                var command = match.Groups["cmd"].Value;
                var project = match.Groups["proj"].Value;

                var git = new GitService(command, project);

                e.Channel.DataReceived += (ss, ee) => git.OnData(ee);
                e.Channel.CloseReceived += (ss, ee) => git.OnClose();
                git.DataReceived += (ss, ee) => e.Channel.SendData(ee);
                git.CloseReceived += (ss, ee) => e.Channel.SendClose(ee);

                git.Start();
            }
            else if (e.ShellType == "client")
            {
                var sshTransportServer = new SSHTransportServer(e.Channel);

                var connection = _cFactory.CreateConnection(sshTransportServer);

                connection.Open();
            }
            else if (e.ShellType == "subsystem")
            {
                // do something more
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}