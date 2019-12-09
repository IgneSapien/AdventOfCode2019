using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Day9
{
    class Program
    {
        static List<long[]> programs;
        static IntCodeVM VM;
        static void Main()
        {
            //Parse programs to be loaded into the VM.
            string[] input = File.ReadLines(@"./Programs/Day9Input.txt").First().Split(',');
            programs = new List<long[]>();
            programs.Add(Array.ConvertAll(input, long.Parse));
            input = File.ReadLines(@"./Programs/Day5Diagnostic.txt").First().Split(',');
            programs.Add(Array.ConvertAll(input, long.Parse));
            //This currently isn't working
            input = File.ReadLines(@"./Programs/CopySelfTest.txt").First().Split(',');
            programs.Add(Array.ConvertAll(input, long.Parse));

            VM = new IntCodeVM(programs);

            while (true)
            {
                //clear down input and outputs to be on the same side
                VM.ClearInputOutput();
                //Might want to get fancy with this in futurue and have a diconatry 
                Console.WriteLine("Program to run (zero indexed)?");
                int program = int.Parse(Console.ReadLine());
                VM.LoadProgram(program);

                Task vmTask = Task.Run(() => VM.Execute());

                long outPut;
                while (!vmTask.IsCompleted)
                {
                    if(VM.InputRequired)
                    {
                        Console.WriteLine("Input Required: ");
                        VM.AddInput(long.Parse(Console.ReadLine()));
                    }
                }
                while (VM.Output.TryTake(out outPut))
                {
                    Console.WriteLine(String.Format("Output: {0}", outPut.ToString()));
                }
                Console.WriteLine("Program Finished");
            }
            //Console.ReadLine();
        }
    }

}
