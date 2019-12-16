using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day14
{
    static class Program
    {
        
        public static double OreN = 1000000000000;


        static void Main()
        {

            Refinary refinary = new Refinary("input1.txt");
            //double test4 = 460664;
            //double test3 = 5586022;

            double ore1Fule = refinary.ProduceFule();
            Console.WriteLine("Ore for one fule: {0}", ore1Fule);
            double lowerBound = Math.Ceiling(OreN / ore1Fule);
            Console.WriteLine("Naive lower bound: {0}", lowerBound);
            //Console.WriteLine("Known test result: {0}", test3);
            refinary.Reset();
            double lowerTest = refinary.ProduceFule(lowerBound);
            Console.WriteLine("Limit: {0}", OreN);
            Console.WriteLine("Lower: {0}", lowerTest);
            refinary.Reset();
            //Console.WriteLine("Test : {0}", refinary.ProduceFule(test3));

            double upperBound = lowerBound * 2;

            while (lowerBound <= upperBound)
            {
                refinary.Reset();
                double mid = Math.Floor((lowerBound + upperBound) / 2);
                double ore = refinary.ProduceFule(mid);
                if (ore < OreN)
                {
                    lowerBound = mid + 1;
                }
                else
                {
                    upperBound = mid - 1;
                }
                if (ore == OreN)
                {
                    Console.WriteLine("jackpot! {0}", mid);
                    break;
                }
                Console.WriteLine("Fule: {0} Ore: {1}", mid, ore);
            }

        }
    }

    public class Refinary
    {
        Dictionary<string, Reaction> reactions;
        Dictionary<string, double> cemicals;
        string inputFile;


        public Refinary(string inputFile)
        {
            this.inputFile = inputFile;
            Reset();
        }

        public void Reset()
        {
            var input = File.ReadLines(inputFile);

            reactions = new Dictionary<string, Reaction>();
            cemicals = new Dictionary<string, double>();

            foreach (string s in input)
            {
                string[] r = s.Split(new[] { "=>" }, StringSplitOptions.None);

                string[] rhs = r[1].Trim().Split(' ');

                KeyValuePair<string, double> ot = new KeyValuePair<string, double>(rhs[1], double.Parse(rhs[0]));

                string[] lhs = r[0].Split(',');
                Dictionary<string, double> cinput = new Dictionary<string, double>();
                foreach (string inc in lhs)
                {
                    string[] i = inc.Trim().Split(' ');
                    cinput.Add(i[1], double.Parse(i[0]));
                }

                Reaction reaction = new Reaction(cinput, ot);
                reactions.Add(rhs[1], reaction);
                cemicals.Add(rhs[1], 0);
            }
        }

        public double ProduceFule(double ammount = 1)
        {
            return React(reactions["FUEL"], ammount);
        }

        public double React(Reaction reaction, double ammount = 1)
        {
            double ore = 0;
            double sapre = cemicals[reaction.Output.Key];
            //Console.WriteLine("Making {0} of {1}", ammount, reaction.Output.Key);

            if (sapre > 0)
            {
                //This is dumb but it's late
                //And of coruse it's what ends up broken!
                //Making 17 of WXHJF
                //92 already exist need 75 leaving 150
                if(sapre >= ammount)
                {
                    cemicals[reaction.Output.Key] -= ammount;
                    ammount = 0;
                }
                else
                {
                    cemicals[reaction.Output.Key] = 0;
                    ammount -= sapre;
                }

                //Console.WriteLine("{0} already exist need {1} leaving {2}", sapre, ammount, cemicals[reaction.Output.Key]);
                if(ammount == 0)
                {
                    //Console.WriteLine("No reaction needed");
                    //Console.WriteLine("{0} ore used", ore);
                    return ore;
                }
            }
            //Console.WriteLine("One reactions of {0} makes {1}", reaction.Output.Key, reaction.Output.Value);
            double rNum = Math.Ceiling(ammount / reaction.Output.Value);
            double leftOver = (reaction.Output.Value * rNum) - ammount;
            //Console.WriteLine("{0} reactions required to make {1} leaving {2} left over", rNum, rNum * reaction.Output.Value, leftOver);


            foreach (KeyValuePair<string, double> cem in reaction.Input)
            {
                if (cem.Key == "ORE")
                {
                    ore += cem.Value * rNum;
                    continue;
                }

                ore += React(reactions[cem.Key], cem.Value * rNum);
            }

            //Console.WriteLine("{0} ore used to make {1} {2}", ore, reaction.Output.Key, reaction.Output.Value * rNum);
            cemicals[reaction.Output.Key] += leftOver;
            return ore;
        }
    }

    public class Reaction
    {
        public Dictionary<string, double> Input { get; set; }
        public KeyValuePair<string, double> Output { get; set; }

        public Reaction(Dictionary<string, double> input, KeyValuePair<string, double> output)
        {
            Input = input;
            Output = output;

        }

       

    }

}
