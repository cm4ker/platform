﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZenPlatform.SSH;

namespace ZenPlatform.Shell.Terminal
{
    internal class TerminalSessionManager
    {
        private readonly List<TerminalSession> _sessions = new List<TerminalSession>();

        public IList<TerminalSession> Sessions
        {
            get => new ReadOnlyCollection<TerminalSession>(_sessions);
        }

        public TerminalSession CreateSession(TerminalSize size)
        {
            PrepareTTermEnvironment();

            TerminalSession session = null;// new TerminalSession(size);
            //session.Finished += OnSessionFinished;

            _sessions.Add(session);
            return session;
        }

        private void PrepareTTermEnvironment()
        {
        }

        private void OnSessionFinished(object sender, EventArgs e)
        {
            var session = sender as TerminalSession;
            _sessions.Remove(session);
        }
    }
}