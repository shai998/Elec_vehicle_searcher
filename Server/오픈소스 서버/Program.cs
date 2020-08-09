using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 오픈소스_서버
{
    class Program
    {
        static void Main(string[] args)
        {
            wbServer server = new wbServer();
            server.ServerRun(9000);
            while(true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("GoodBye");
                    return;
                }
                    
            }
        }
    }
}
