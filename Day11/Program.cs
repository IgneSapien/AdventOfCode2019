using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;

namespace Day11
{
    class Program
    {
        static void Main()
        {
            //Parse programs to be loaded into the VM.
            string[] input = File.ReadLines(@"./Programs/Input.txt").First().Split(',');
            List<long[]> programs = new List<long[]>();
            programs.Add(Array.ConvertAll(input, long.Parse));


            Robot bot = new Robot((0, 0), programs);

            bot.StartPainting();

            Console.ReadLine();
        }
    }
}
