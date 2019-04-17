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

        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }

        public static readonly char[] FORBIDDEN_CHARS = new char[] {'\r', '\n', '\t'};
        public static readonly int WINDOW_WIDTH  = Console.WindowWidth;
        public static readonly int WINDOW_HEIGHT = Console.WindowHeight-1;

        private long lastRefresh;
        private bool running;
        private int cursorX;
        private int cursorY;
        private char[] buffer;

        public Program()
        {
            lastRefresh = 0;
            running = true;
            cursorX = 0;
            cursorY = 0;
            buffer = new char[WINDOW_HEIGHT * WINDOW_WIDTH];
            for (int i = 0; i < buffer.Length; i++) buffer[i] = ' ';
            Console.CursorVisible = false;
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
                                    cursorY = 0;
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
                                if(cursorY >= WINDOW_HEIGHT)
                                {
                                    cursorY = WINDOW_HEIGHT;
                                }
                            }
                            break;
                    }

                    Redraw();
                }
                if(lastRefresh + 1000 < Millis)
                {
                    Redraw();
                    lastRefresh = Millis;
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
            }

            string titleText = "# Terminal Text Editor v0.01";
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(titleText + new string(' ', WINDOW_WIDTH - titleText.Length));

            Console.SetCursorPosition(0, 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(builder.ToString());

            Console.SetCursorPosition(cursorX%WINDOW_WIDTH, (cursorY%WINDOW_HEIGHT)+1);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(GetCharAt(cursorX, cursorY));

            Console.SetCursorPosition(0, 0);
        }
    }
}
