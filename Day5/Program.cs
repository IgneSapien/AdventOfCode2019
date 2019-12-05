using System;
using System.IO;
using System.Linq;

namespace Day5
{
    class Program
    {
        static int[] program;
        static IntCodeVM VM;
        static void Main()
        {
            string[] input = File.ReadLines(@"./Programs/diagnostic.txt").First().Split(',');
            program = Array.ConvertAll(input, int.Parse);
            VM = new IntCodeVM(program);

            while (true)
            {
                VM.LoadProgram(program);
                VM.Execute();
                Console.WriteLine("Program has Finished Executing");
            }
        
        }


    }
}
