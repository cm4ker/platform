using System;

namespace AsemblyTester
{
    class Program
    {
        static void Main(string[] args)
        {
            CompileNamespace.__cmd_HelloFromServer.ClientCallProc(10);
        }
    }
}