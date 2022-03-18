using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquila.Core
{
    /// <summary>
    /// A session state.
    /// </summary>
    public enum AqSessionState
    {
        /// <summary>
        /// The default, closed, state.
        /// </summary>
        Default = Closed,

        /// <summary>
        /// Session was not started yet or was closed already.
        /// </summary>
        Closed = 0,

        /// <summary>
        /// Session has started.
        /// </summary>
        Started = 1,

        /// <summary>
        /// Session is in progress of loading or closing.
        /// </summary>
        InProgress = 2,
    }

    /// <summary>
    /// A Aquila session handler providing basic session operations.
    /// </summary>
    public abstract class AqSessionHandler
    {
        /// <summary>
        /// Dummy session item keeping .NET session object alive.
        /// Used by derived class.
        /// </summary>
        protected const string DummySessionItem = "Aquila_DummySessionKeepAliveItem(\uffff)";

        /// <summary>
        /// Name of PHP <c>SID</c> constant to be set when starting session.
        /// </summary>
        public static string SID_Constant => "SID";

        /// <summary>
        /// Gets or sets the session name.
        /// </summary>
        public abstract string GetSessionName(IAqHttpContext webctx);

        /// <summary>
        /// Gets or sets the session name.
        /// </summary>
        public abstract bool SetSessionName(IAqHttpContext webctx, string name);

        /// <summary>
        /// Gets this handler name.
        /// </summary>
        public abstract string HandlerName { get; }

        /// <summary>
        /// Frees the session.
        /// Next time a new (empty) sesison should be created.
        /// </summary>
        /// <param name="webctx">Current web context.</param>
        public abstract void Abandon(IAqHttpContext webctx);

        /// <summary>
        /// Gets the session ID (SID constant).
        /// </summary>
        public abstract string GetSessionId(IAqHttpContext webctx);

        /// <summary>
        /// Sets the session ID (SID constant).
        /// </summary>
        public virtual bool SetSessionId(IAqHttpContext webctx, string newid) => false;

        /// <summary>
        /// Gets value indicating the sessions are configured and available to use.
        /// </summary>
        public virtual bool IsEnabled(IAqHttpContext webctx) => true;

        /// <summary>
        /// Starts the session if it is not started yet.
        /// </summary>
        public virtual bool StartSession(AqContext ctx, IAqHttpContext webctx)
        {
            // checks and changes session state:
            if (webctx.SessionState != AqSessionState.Closed) return false;
            webctx.SessionState = AqSessionState.InProgress;

            try
            {
            }
            catch
            {
                webctx.SessionState = AqSessionState.Closed;
                return false;
            }
            finally
            {
                //
                webctx.SessionState = AqSessionState.Started;
            }

            //
            return true;
        }

        /// <summary>
        /// Discard session array changes and finish session.
        /// </summary>
        public virtual void AbortSession(AqContext ctx, IAqHttpContext webctx)
        {
            if (webctx.SessionState != AqSessionState.Started) return;
            webctx.SessionState = AqSessionState.InProgress;

            try
            {
                // TODO: clear $_SESSION ? 
            }
            finally
            {
                webctx.SessionState = AqSessionState.Closed;
            }
        }

        /// <summary>
        /// Closes the session and either persists the session data or abandons the session.
        /// </summary>
        public virtual void CloseSession(AqContext ctx, IAqHttpContext webctx, bool abandon)
        {
            if (webctx.SessionState != AqSessionState.Started) return;
            webctx.SessionState = AqSessionState.InProgress;

            //
            try
            {
                if (abandon)
                {
                    Abandon(webctx);
                }
                else
                {
                    //Persist(webctx, ctx.Session ?? PhpArray.Empty);
                }
            }
            finally
            {
                webctx.SessionState = AqSessionState.Closed;
            }
        }
    }
}