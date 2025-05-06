using System;
using System.Threading;

namespace Server
{
    class Server
    {
        static GameServer gameServer;

        public static void Main(string[] args)
        {
            Console.WriteLine("[Server] Start");
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                gameServer?.Dispose();
                Console.WriteLine("[Server] Release");
            };

            gameServer = new GameServer();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
