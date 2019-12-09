using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day9
{
    class Program
    {
        static long[] program;
        static IntCodeVM VM;
        static void Main()
        {
            string[] input = File.ReadLines(@"./Programs/CopySelfTest.txt").First().Split(',');
            program = Array.ConvertAll(input, long.Parse);


            VM = new IntCodeVM(program);

            while (true)
            {
                VM.LoadProgram();
                Task vmTask = Task.Run(() => VM.Execute());

                //Console.WriteLine("Intial Input: ");
                //VM.Input.Add(long.Parse(Console.ReadLine()));

                long outPut;
                while (!vmTask.IsCompleted)
                {
                    //if (VM.Output.TryTake(out outPut))
                    //{
                    //    Console.WriteLine(String.Format("Output: {0}", outPut.ToString()));
                    //}
                }
                while (VM.Output.TryTake(out outPut))
                {
                    Console.WriteLine(String.Format("Output: {0}", outPut.ToString()));
                }
                Console.WriteLine("Program Finsihed");

                //copy to self not working.
                Console.WriteLine("Press key to run again");
                Console.ReadKey();
            }
            //Console.ReadLine();
        }
    }

}
