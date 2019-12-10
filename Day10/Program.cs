using System;
using SadRogue.Primitives;

namespace Day10
{
    class Program
    {
        //Using SadRogue.Primitives Alpha

        static void Main(string[] args)
        {
            Point pA = new Point(0, 0);
            Point pB = new Point(1, 2);
            Point pC = new Point(2, 3);
            Point pD = new Point(3, 3);
            int dAB = Dist(pA, pB);
            int dAC = Dist(pA, pC);
            int dAD = Dist(pA, pD);

            for (int i = 0; i < dAB; i++)
            {

            }
            
            Console.WriteLine(dAB.ToString());
            Console.WriteLine(dAC.ToString());
            Console.WriteLine(dAD.ToString());
            Console.ReadLine();
        }

        private static int Dist(Point p1, Point p2)
        {
            Point v = p1 - p2;
            int d = Math.Max(Math.Abs(v.X), Math.Abs(v.Y));
            return d;
        }
    }
}
