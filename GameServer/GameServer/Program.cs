using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        private static bool isRunning; 
        static void Main(string[] args)
        {
            Console.Title = "Server";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            
            Server.Start(4, 26950);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Mian thread started. Running at {Constants.k_ticksPerSecond} ticks per second");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.k_millisecondsPerTick);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}