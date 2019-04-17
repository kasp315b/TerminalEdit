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

        public static readonly int WINDOW_WIDTH  = Console.WindowWidth-1;
        public static readonly int WINDOW_HEIGHT = Console.WindowHeight-2;

        private bool running;
        private int cursorX;
        private int cursorY;
        private char[] buffer;

        public Program()
        {
            running = true;
            cursorX = 0;
            cursorY = 0;
            buffer = new char[WINDOW_HEIGHT * WINDOW_WIDTH];
            for (int i = 0; i < buffer.Length; i++) buffer[i] = ' ';
            Run();
        }

        public void Run()
        {
            while(running)
            {
                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    ConsoleKey key = keyInfo.Key;
                    char keyChar = keyInfo.KeyChar;

                    switch(key)
                    {
                        case ConsoleKey.RightArrow:
                            cursorX = Math.Min(cursorX + 1, WINDOW_WIDTH);
                            break;
                        case ConsoleKey.LeftArrow:
                            cursorX = Math.Max(cursorX - 1, 0);
                            break;
                        case ConsoleKey.UpArrow:
                            cursorY = Math.Max(cursorY - 1, 0);
                            break;
                        case ConsoleKey.DownArrow:
                            cursorY = Math.Min(cursorY + 1, WINDOW_HEIGHT);
                            break;
                        case ConsoleKey.Enter:
                            cursorX = 0;
                            goto case ConsoleKey.DownArrow;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                        case ConsoleKey.Backspace:
                            cursorX--;
                            if(cursorX < 0)
                            {
                                cursorY--;
                                if(cursorY < 0)
                                {
                                    cursorY = WINDOW_HEIGHT;
                                }
                                for(cursorX = WINDOW_WIDTH; GetCharAt(cursorX, cursorY) == ' ' && cursorX > 0; cursorX--);
                                if(cursorX != 0) goto case ConsoleKey.RightArrow;
                                break;
                            }
                            SetCharAt(cursorX, cursorY, ' ');
                            break;
                        default:
                            SetCharAt(cursorX, cursorY, keyChar);
                            cursorX++;
                            if(cursorX > WINDOW_WIDTH)
                            {
                                cursorY++;
                                cursorX = 0;
                                if(cursorY > WINDOW_HEIGHT)
                                {
                                    cursorY = 0;
                                }
                            }
                            break;
                    }

                    Redraw();
                }
                Thread.Sleep(10);
            }
        }

        private char GetCharAt(int x, int y)
        {
            return buffer[GetIndexAt(x, y)];
        }

        private void SetCharAt(int x, int y, char c)
        {
            buffer[GetIndexAt(x, y)] = c;
        }

        private int GetIndexAt(int x, int y)
        {
            return Math.Min(Math.Max(x + y * WINDOW_WIDTH, 0), WINDOW_WIDTH * WINDOW_HEIGHT - 1);
        }

        public void Redraw()
        {
            StringBuilder builder = new StringBuilder();
            for(int y = 0; y < WINDOW_HEIGHT; y++)
            {
                for(int x = 0; x < WINDOW_WIDTH; x++)
                {
                    builder.Append(GetCharAt(x, y));
                }
                builder.Append("\r\n");
            }

            string titleText = "Terminal Text Editor";
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(titleText + new string(' ', WINDOW_HEIGHT - titleText.Length));

            Console.SetCursorPosition(0, 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(builder.ToString());

            Console.SetCursorPosition(cursorX, cursorY+1);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(GetCharAt(cursorX, cursorY));

            Console.SetCursorPosition(WINDOW_WIDTH, WINDOW_HEIGHT);
        }
    }
}
