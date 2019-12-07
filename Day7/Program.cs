using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Day7
{
    class Program
    {
        static int[] program;

        static List<Amp> amps;

        static IEnumerable<IEnumerable<int>> combos;

        static void Main()
        {
            string[] input = File.ReadLines(@"input.txt").First().Split(',');
            program = Array.ConvertAll(input, int.Parse);
            int numberOfAmps = 5;
            combos = GetPermutations(Enumerable.Range(5, numberOfAmps), numberOfAmps);
            SetUpAmps(numberOfAmps);

            RunTest();

            Console.ReadLine();
        }


        static async void RunTest()
        {
            int max = 0;
            string maxPhase = "00000";

            //IEnumerable<int> c = new int[] { 9, 8, 7, 6, 5 };

            foreach (IEnumerable<int> c in combos)
            {
                foreach (Amp a in amps)
                {
                    a.ampVM.ClearInputOutput();
                }

                String test = "";

                for (int i = 0; i < c.Count(); i++)
                {
                    test += c.ElementAt(i).ToString();
                    int p = c.ElementAt(i);
                    amps[i].Phase = p;
                    amps[i].AmpInput(p);
                }

                Console.WriteLine(String.Format("Testing: {0}", test));

                //Set input for first amp
                amps.First().AmpInput(0);

                var tasks = new List<Task>();
                 
                foreach(Amp a in amps)
                {
                    Task ampTask = Task.Run(() => a.RunAmp());
                    tasks.Add(ampTask);
                }

                await Task.WhenAll(tasks);

                if (!amps.Last().AmpOutput(out int r))
                {
                    r = amps.Last().LastOutput;
                }

                Console.WriteLine(String.Format("Output: {0}", r.ToString()));

                if(r > max)
                {
                    max = r;
                    maxPhase = test;
                }

               
            }

            Console.WriteLine(String.Format("Max Thrust: Phase: {0} Thrust:{1}", maxPhase, max.ToString()));
        }


        private static void SetUpAmps(int n)
        {
            amps = new List<Amp>();

            for (int i = 0; i < n; i++)
            {
                amps.Add(new Amp(new IntCodeVM(program)));
            }

            for (int i = 1; i < n; i++)
            {
                amps[i].LinkedAmp = amps[i - 1];
            }

            amps.First().LinkedAmp = amps.Last();
        }


        //Taken wholesale from https://stackoverflow.com/questions/756055/listing-all-permutations-of-a-string-integer 
        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
