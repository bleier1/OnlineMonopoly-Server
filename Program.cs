// The main program file. The server starts from here.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonopoly
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize some server stuff.
            Server server = new Server();
            server.SetUpServer();
            // The server is able to be shut down by a simple keystroke, so... try not to do that...
            Console.ReadLine();
        }
    }
}
