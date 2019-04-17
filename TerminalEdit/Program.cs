using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TerminalEdit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program();
        }

        private bool running;
        private int cursorX;
        private int cursorY;

        public Program()
        {
            running = true;
            cursorX = 0;
            cursorY = 0;
            Run();
        }

        public void Run()
        {
            while(running)
            {
                if(Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                }
                Thread.Sleep(20);
            }
        }
    }
}
