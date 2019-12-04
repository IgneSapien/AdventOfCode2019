using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Day3
{
    class Program
    {
        static Dictionary<(int, int), Point> points = new Dictionary<(int, int), Point>();
        static List<int> cross = new List<int>();
        static void Main()
        {
            string[] inputA = File.ReadLines("Task1a.txt").First().Split(',');
            string[] inputB = File.ReadLines("Task1b.txt").First().Split(',');

            RunWire(inputA,'a');
            RunWire(inputB,'b');

            ClosetsCross();

            Console.ReadLine();
        }

        static void RunWire(string[] wire, char type)
        {
            int x = 0;
            int y = 0;
            int steps = 0;
            foreach(string s in wire)
            {
                char d = char.Parse(s.Substring(0, 1));
                int c = int.Parse(s.Substring(1));

                for (int i = 0; i < c; i++)
                {
                    steps++;
                    switch (d)
                    {
                        case 'U': y++; break;
                        case 'D': y--; break;
                        case 'R': x--; break;
                        case 'L': x++; break;
                        default: break;
                    };


                    Point p;
                    if (!points.TryGetValue((x, y), out p))
                    {
                        p = new Point(x, y);
                        p.WireCrossed += PointEventHandler;
                        points.Add((x, y), p);
                    }

                    if (type == 'a')
                    {
                        p.a = steps;
                    }
                    else
                    {
                        p.b = steps;
                    }
                }
               
            }
        }

        static void ClosetsCross()
        {
            Console.WriteLine("Min cross: {0}", cross.Min());
        }

        static void PointEventHandler(int x, int y, int s)
        {
            Console.WriteLine("Wires crossed at {0},{1} Distance {2}", x,y,s);
            cross.Add(s);
        }
    }

    class Point
    {
        public int x { get; private set; }
        public int y { get; private set; }

        bool _a;
        int _ai;
        public int a
        {
            get
            {
                return _ai;
            }

            set
            {
                _a = true;
                _ai = value;
                check();
            }
        }

        bool _b;
        int _bi;
        public int b
        {
            get
            {
                return _bi;
            }

            set
            {
                _b = true;
                _bi = value;
                check();
            }
        }

        private void check()
        {
            if (_a && _b)
            {
                WireCrossed(x, y, _ai + _bi);
            }
        }

        public event Action<int, int, int> WireCrossed;

        public Point (int x, int y)
        {
            this.x = x;
            this.y = y;
            _a = false;
            _b = false;
            _ai = 0;
            _bi = 0;
        }
    }
}
