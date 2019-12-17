using System;
using System.Collections.Generic;
using System.Linq;

namespace Day16
{
    class Program
    {
        static void Main()
        {
            //string signalString = "59738476840592413842278931183278699191914982551989917217627400830430491752064195443028039738111788940383790187992349338669216882218362200304304999723624146472831445016914494176940890353790253254035638361091058562936884762179780957079673204210602643442603213181538626042470133454835824128662952579974587126896226949714610624975813583386749314141495655816215568136392852888525754247201021383516228214171660111826524421384758783400220017148022332694799323429711845103305784628923350853888186977670136593067604033507812932778183786479207072226236705355778245701287481396364826358903409595200711678577506495998876303181569252680220083046665757597971122614";
            //Part2 tests
            string signalString = "03036732577212944063491565474664";

            string basePatternString = "0,1,0,-1";
            int[] basePattern = basePatternString.Split(",").Select(c => int.Parse(c)).ToArray();
            int offset = int.Parse(signalString.Substring(0, 7));

            //Part1(signalString, basePattern);

            //part2
            //Thanks to being pointed in the right direction I can see that past the halfway point in the signal there's a pattern that exists from the end
            //That pattern is the sum of the digits from that point in the previous phase 
            //1   1   1   1   1   1   1   1
            //8   7   6   5   4   3   2   1
            //36  28  21  15  10  6   3   1
            //Given we've got a 7 digit offset for a 7 didigt len single our answer is in the second half
            //As such we can work out the next phase easily based on the previous phase only 
            //And I think we only need the signal from the offest 

            //TODO: Make this work, I'm buggering it up somewhere. Will come back to it. 

            List<double> signalList = signalString.Select(c => double.Parse(c.ToString())).ToList();
            List<double> newList = new List<double>();

            int requiredLen = (signalList.Count() * 10000) - offset;


            while(newList.Count() <= requiredLen)
            { 
                newList = newList.Concat(signalList).ToList();
            }

            newList.RemoveRange(0, newList.Count() - requiredLen);

            double[] signal = newList.ToArray();

            for (int i = 0; i < 100; i++)
            {
                for (int s = signal.Count() - 2; s >= 0; s--)
                {
                    double r = signal[s];
                    double r2 = signal[s + 1];
                    signal[s] = r + r2;
                }
            }

            for (int i = 0; i < signal.Count(); i++)
            {
                signal[i] = signal[i] % 10;
            }


            Console.WriteLine("Message: {0}{1}{2}{3}{4}{5}{6}{7}", signal[0], signal[1], signal[2], signal[3], signal[4], signal[5], signal[6], signal[7]);

        }

        private static void Part1(string signalString, int[] basePattern)
        {
            //part1
            int[] signal = signalString.Select(c => int.Parse(c.ToString())).ToArray();
            //Console.WriteLine("singal [{0}]", string.Join(", ", signal));
            //Console.WriteLine("basePattern [{0}]", string.Join(", ", basePattern));
            Console.WriteLine("{0}", string.Join(", ", signal));
            for (int i = 0; i < 100; i++)
            {
                AdjustSignal(ref signal, basePattern);
                Console.WriteLine("{0}", string.Join(", ", signal));
            }

            Console.WriteLine("Output signal [{0}]", string.Join("", signal));
        }

        private static void AdjustSignal(ref int[] signal, int[] basePattern)
        {
            int signalLen = signal.Length;
           
            for (int i = 0; i < signalLen; i++)
            {
                int[] pattern = AdjustPattern(basePattern, i + 1, signalLen);
                int sum = 0;
                for (int s = i; s < signalLen; s++)
                {
                    sum += signal[s] * pattern[s];
                }

                //signal[i] = Math.Abs(sum) % 10;
                signal[i] = Math.Abs(sum);
            }
        }

        static int[] AdjustPattern(int[] basePattern, int ittarion, int signalLen)
        {
            int baseLen = basePattern.Length;
            List<int> newPattern = new List<int>();

            for (int p = 0; p < baseLen; p++)
            {
                for (int i = 0; i < ittarion; i++)
                {
                    newPattern.Add(basePattern[p]);
                }
            }

            while(newPattern.Count <= signalLen)
            {
                newPattern = newPattern.Concat(newPattern).ToList();
            }
            newPattern.RemoveAt(0);

            return newPattern.ToArray();
        }
    }
}
