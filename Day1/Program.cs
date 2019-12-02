using System;
using System.Collections.Generic;
using System.IO;

namespace Day1
{
    class Program
    {
        static void Main()
        {
            int t = 0;
            foreach (string s in File.ReadLines("Task1.txt"))
            {
                t += ReFuleCost(int.Parse(s));
            }
            Console.WriteLine(t.ToString());
            Console.ReadLine();
        }

        private static int FuleCost(int w)
        {
            return(w / 3) - 2;
        }

        private static int ReFuleCost(int w)
        {
            if (w <= 8)
            {
                return 0;
            }
            int r = (w / 3) - 2;
            return r + ReFuleCost(r);
        }
    }
}
