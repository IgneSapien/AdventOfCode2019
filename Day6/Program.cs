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
            var input = File.ReadLines("Task1.txt").ToList();

            //This whole bit feels horribly inefficent but it works. 
            bodies = new List<string[]>();
            foreach (string s in input)
            {
                var p = s.Split(')');
                bodies.Add(p);
            }
            root = new Body("COM");
            BuildTree("COM");

            //PART ONE
            Console.WriteLine(root.SumChild().ToString());

            //PART TWO
            //Could do some more advance pathfinding but given the constraints of the propblem 1) Everything hangs off the root node 2) each node can only have one parent 
            //Then we know the shortest path from YOU to SAN is the first node their paths to the root intersect. 

            var youPath = root.Find("YOU").PathToRoot();
            var sanPath = root.Find("SAN").PathToRoot();
            var intersect = youPath.Intersect(sanPath);

            int r = youPath.Except(intersect).Count() + sanPath.Except(intersect).Count(); 

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
