using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace Day4
{
    class Program
    {
        static void Main()
        {
            int c = 0;
            for (int p = 357253; p <= 892942; p++)
            {
                if (Check(p.ToString()))
                {
                    c++;
                }
            }
            Console.WriteLine(c.ToString());

            //Console.WriteLine(Check("122345").ToString());
            //Console.WriteLine(Check("111111").ToString());
            //Console.WriteLine(Check("223450").ToString());
            //Console.WriteLine(Check("123789").ToString());
            //Console.WriteLine(Check("112233").ToString());
            //Console.WriteLine(Check("123444").ToString());
            //Console.WriteLine(Check("113444").ToString());

            Console.ReadLine();
        }

        private static bool Check(string pass)
        {
            var check = pass.OrderBy(i => i);
            bool rise = Enumerable.SequenceEqual(pass, check);
            if(!rise)
            {
                return false;
            }
            //this works because the same numbers have to next to each other due to the above. It would fail if we were allowed 11234511
            if(check.GroupBy(i => i).Where(g => g.Count() == 2).Count() > 0)
            {
                return true;
            }

            return false;
        }



    }
}
