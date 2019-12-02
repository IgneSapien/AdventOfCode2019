using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        
        static int[] program;
        static int[] newProgram;
        static IntCodeVM VM;
        static void Main()
        {
            string[] input = File.ReadLines("Task1.txt").First().Split(',');
            program = Array.ConvertAll(input, int.Parse);
            VM = new IntCodeVM(program);
            int[] answer = Test();

            Console.WriteLine(String.Format("N: {0} v: {1}, Answser:{2}", answer[0], answer[1], 100 * answer[0] + answer[1]));
            Console.ReadLine();
        }

        private static int[] Test()
        {
            for (int n = 0; n <= 99; n++)
            {
                for (int v = 0; v <= 99; v++)
                {
                    newProgram = (int[])program.Clone();
                    newProgram[1] = n;
                    newProgram[2] = v;
                    VM.LoadProgram(newProgram);
                    VM.Execute();
                    if (VM.Memory[0] == 19690720)
                    {
                        return new int[2] { n, v };
                    }
                }
            }

            return new int[2] { -1, -1 };
        }

        
    }
}
