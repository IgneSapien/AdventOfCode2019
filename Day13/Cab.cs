using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using System.Linq;
using System.Threading;

namespace Day13
{

    public class Cab
    {
        IntCodeVM Brain;
        Task vmTask;

        Dictionary<Point, long> screen;

        Point ball;
        Point paddle;

        long score;

        public Cab(List<long[]> intialPrograms)
        {
            Brain = new IntCodeVM(intialPrograms);
            Brain.LoadProgram();
            ball = new Point(0, 0);
            paddle = new Point(0, 0);
        }

        public void StartGame()
        {
            screen = new Dictionary<Point, long>();
            score = 0;

            screen.Clear();
            vmTask = Task.Run(() => Brain.Execute());
            while (!vmTask.IsCompleted)
            {
                if (Brain.WatingForInput)
                {
                    ReadScreen();
                    DrawScreen();
                    AIinput();
                    Thread.Sleep(10);
                }

            }
            ReadScreen();
            DrawScreen();
            Console.WriteLine("VM task compelte");
        }

        void AIinput()
        {
            int i = 0;
            if(paddle.X < ball.X)
            {
                i = 1;
            }
            if(paddle.X > ball.X )
            {
                i = -1;
            }

            Brain.WriteToInput.TryWrite(i);
        }

        void ReadInput(ConsoleKeyInfo i)
        {
            switch (i.Key)
            {
                case ConsoleKey.UpArrow : Brain.WriteToInput.TryWrite(0); break;
                case ConsoleKey.LeftArrow: Brain.WriteToInput.TryWrite(-1); break;
                case ConsoleKey.RightArrow: Brain.WriteToInput.TryWrite(1); break;

                default:
                    Console.WriteLine("Unknown key: please use arrow keys, up == stay");
                    break;
            };
        }

        void ReadScreen()
        {
            List<long> outp = new List<long>();
            while (Brain.ReadFromOutput.TryRead(out long outPut))
            {
                outp.Add(outPut);
            }
            int olen = outp.Count();
            for (int ti = 0; ti < olen; ti += 3)
            {
                long[] t = new long[3];
                for (int i = 0; i < 3; i++)
                {
                    t[i] = outp[ti + i];
                }
                Point p = new Point((int)t[0], (int)t[1]);
                if (screen.ContainsKey(p))
                {
                    screen[p] = t[2];
                }
                else {
                    screen.Add(p, t[2]);
                }
            }
        }

        void DrawScreen()
        {
            Console.Clear();
            screen.TryGetValue((-1, 0), out long newscore);
            score = newscore;
            Console.WriteLine("Score: {0}", score.ToString());
            screen.Remove((-1, 0));

            int minX = screen.Keys.Select(x => x.X).Min();
            int maxX = screen.Keys.Select(x => x.X).Max();

            int maxY = screen.Keys.Select(y => y.Y).Max();
            int minY = screen.Keys.Select(y => y.Y).Min();

            //int width = Math.Abs(minY) + Math.Abs(maxX);
            //int height = Math.Abs(minY) + Math.Abs(maxY);
            int bCount = 0;
            for (int y = minY; y <= maxY; y++)
            {
                StringBuilder sb = new StringBuilder();
                for (int x = minX; x <= maxX; x++)
                {
                    if (screen.TryGetValue((x, y), out long output))
                    {

                        var t = output switch
                        {
                            0 => " ",
                            1 => "|",
                            2 => "#",
                            3 => "=",
                            4 => "O",
                            _ => "?",
                        };

                        //if (output == 2)
                        //    bCount++;

                        if(output == 3)
                        {
                            paddle = new Point(x, y);
                        }
                        if(output == 4)
                        {
                            ball = new Point(x, y);
                        }

                        sb.Append(t);
                    }
                    else
                    {
                        sb.Append("!");
                    }
                    
                }
                Console.WriteLine(sb.ToString());
            }
        }
    }
}
