using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZenPlatform.ServerRPC
{
    public class ThreadContext_Draft
    {
        /*
         
         Thread start: 
            Set context
         Thread end:
            Drop context if next circumstance is good: 
                1) This is Parent thread
                2) All children threads is end
                3) The user is not available         
         
         
         (contextInfo) => 
         {
            //Here Created new context from info
            
            var context = ContextFactory.Create(contextInfo);
            
            GlobalStorage.Contexts.Save(Thread.CurrentThread.ManagedThreadId, context)
            
            try
            {
                SomeJob();
            }
            finally
            {
                 GlobalStorage.Contexts.Remove(Thread.CurrentThread.ManagedThreadId);                
            }
            
            
            in every compiled method from platform you can get the context
            
            GlobalStorage.Contexts.Get(Thread.CurrentThread)
            
         }
         
         
         SessionContext
         {
            ConnectionsToTheDatabase

            SuperAdminMode(false/true)
            
            UserName,
            UserRoles,
            
            Etc...
         }
         
         
         public super void StoreInvoice()
         {
            var ivn = $D.InvoiceCreate();
            inv.Date = Now();
            inv.SomeField = 1;
            inv.Save();
            
            var user = GetUser();
                        
            context super
            {
                inv.Save();
                
                //any code without filtering
                //Context.SuperUser = true;                    
            }
            
            // for this user MUST have rights Impersonate
            execute user
            {
                // impersonate as user
                //Context current user = user
                //in log journal it will be as
                // user1 as user2...
                        
            }
         }

         
         */


        /// <summary>
        /// Provides a way to set contextual data that flows with the call and 
        /// async context of a test or invocation.
        /// </summary>
        public static class CallContext
        {
            static ConcurrentDictionary<string, AsyncLocal<object>> state =
                new ConcurrentDictionary<string, AsyncLocal<object>>();

            /// <summary>
            /// Stores a given object and associates it with the specified name.
            /// </summary>
            /// <param name="name">The name with which to associate the new item in the call context.</param>
            /// <param name="data">The object to store in the call context.</param>
            public static void SetData(string name, object data) =>
                state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

            /// <summary>
            /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
            /// </summary>
            /// <param name="name">The name of the item in the call context.</param>
            /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
            public static object GetData(string name) =>
                state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
        }

        public static async Task Main2()
        {
        }

        //Sample 02: Define Task/Wait Callback function
        private static void TaskCallBack(object threadNumber)
        {
            Console.WriteLine(
                $"Internal Thread: {Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.IsThreadPoolThread}");
        }
    }
}