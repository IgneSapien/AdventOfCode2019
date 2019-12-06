using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Day6
{
    class Program
    {
        static List<string[]> bodies;
        static Body root;
        static void Main()
        {
            //FOR PART2
            //Should I find the first common orbit, rebase the tree and then sum the jumps from each? Do
            //If I move the county stuff into the nodes no need to rebase

            var input = File.ReadLines("Task1.txt").ToList();

            bodies = new List<string[]>();
            foreach (string s in input)
            {
                var p = s.Split(')');
                bodies.Add(p);
            }

            root = new Body("COM");
            BuildTree("COM");

            //Console.WriteLine(root.SumChild().ToString());


            var youPath = root.Find("YOU").PathToRoot();
            var sanPath = root.Find("SAN").PathToRoot();
            var intersect = youPath.Intersect(sanPath);

            //foreach(string s in intersect)
            //{
            //    Console.WriteLine(s);
            //}

            Body newRoot = root.Find(intersect.First());

            newRoot.MakeRoot();

            int r = newRoot.Find("YOU").SumChild();
            r += newRoot.Find("SAN").SumChild();
            //This method includes "jumps" to YOU and SANs so we'd be off by 2.
            r -= 2;
            Console.WriteLine(r.ToString());

            Console.ReadLine();
        }

        private static void BuildTree(string ID)
        {
            var children = bodies.Where(b => b[0] == ID);
            Body parent = root.Find(ID);
            foreach (string[] child in children)
            {
                parent.Add(new Body(child[1]));
                BuildTree(child[1]);
            }
        }
    }
}
