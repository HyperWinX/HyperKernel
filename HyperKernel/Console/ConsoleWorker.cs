using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperKernel.Console
{
    public static class ConsoleWorker
    {
        public static void Work()
        {
            System.Console.Clear();
            System.Console.WriteLine("Welcome to the HyperKernel!");
            System.Console.WriteLine("Type help for commands list.\n");
            while (true)
            {
                System.Console.Write(">>> ");
                string query = System.Console.ReadLine();
                string[] splittedQuery = query.Split(' ');
                RequestPerform.PerformRequest(splittedQuery);
            }
        }
    }
}
