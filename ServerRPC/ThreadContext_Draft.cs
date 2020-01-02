using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;

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


        public static async Task Main()
        {
            var b = new ThreadLocal<DateTime>();
        }
    }
}