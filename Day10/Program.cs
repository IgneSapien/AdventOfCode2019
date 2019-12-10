using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography;
using SadRogue.Primitives;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace Day10
{
    class Program
    {
        //Using SadRogue.Primitives Alpha

        static void Main()
        {
            List<Point> Astroids = new List<Point>();
            var input = File.ReadLines(@"./Inputs/BigTest.txt");
            int inputY = input.Count();
            int inputX = input.First().Length;

            for (int y = 0; y < inputY; y++)
            {
                StringBuilder sb = new StringBuilder();
                string s = input.ElementAt(y);
                for (int x = 0; x < inputX; x++)
                {
                    char c = s[x];
                    sb.Append(c);
                    if (c == '#')
                    {
                        Astroids.Add((x, y));
                    }
                }
                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine();

            //Part1(Astroids);
            //Best roid in part one is 11,11
            //Best roid in Big test is 11,13

            //Point bigLaser = (11, 11);
            Point bigLaser = (11, 13);

            int startX = bigLaser.X;

            //Get perminter. Easier ways to do this but wanted to try using the Rectangle 
            //Rectangle rectangle = new Rectangle((0, 0), (inputX, inputY));
            //var perimeter = rectangle.PerimeterPositions();
            //List<Point> shift = new List<Point>();
            //foreach(Point p in perimeter)
            //{
            //    if(p.Y == 0 && p.X < startX)
            //    {
            //        shift.Add(p);
            //    }
            //}
            //List<Point> targets = perimeter.Except(shift).ToList();
            //targets.AddRange(shift);

            bool FIREEVERYTHING = true;
            List<Point> blownUp = new List<Point>();

            while(FIREEVERYTHING)
            {
                List<Point> blownUpThisRound = new List<Point>();
                for(double d = 0; d <= 360; d++)
                {
                    double a = d + 90;
                    AngleSlope slope = new AngleSlope(bigLaser, a,(inputX,inputY));
                    //Get all whole points on slope
                    List<Point> slopePoints = slope.WholePointsOnSlope();
                    //Get any astroids on this point
                    //This could be much more effecent but meh
                    List<Point> comp = slopePoints.Intersect(Astroids).ToList();
                    
                    //IT'S A HIT?!
                    if (comp.Count() > 0)
                    {
                        //if there's more than one order by distinace 
                        if (comp.Count() > 1)
                        {
                            comp = comp.OrderBy(x => Dist(slope.StartP, x)).ToList();
                        }
                        //Check if the first visable astroid is the one we're looking for add it to the list if it is
                        //If we know this astroid can't see another then we know that astroid can't see this.
                        //We can also roll that knowlage down the list
                        //But this would need us to set up everything first which I don't think is currently required. 
                        //I could spin each of these off into a Task as well but it only takes a few seconds to run on the input.
                        Point hit = comp.First();
                        Console.WriteLine(String.Format("{0} went boom", hit.ToString()));
                        blownUpThisRound.Add(comp.First());
                    }
                }
                blownUp.AddRange(blownUpThisRound);
                Astroids = Astroids.Except(blownUpThisRound).ToList();
                if(Astroids.Count() < 0)
                {
                    FIREEVERYTHING = false;
                }
            }
            Console.WriteLine(String.Format("200th Blowny upy: {0}", blownUp[200].ToString()));
            Console.ReadLine();
        }

        private static void Part1(List<Point> Astroids)
        {
            Dictionary<Point, int> roidCount = new Dictionary<Point, int>();
            foreach (Point seekerRoid in Astroids)
            {
                List<Point> visableRoids = new List<Point>();
                foreach (Point roidToFind in Astroids)
                {

                    if (seekerRoid == roidToFind)
                    {
                        //there's easier ways of doing this but I don't want to mess with the main list
                        continue;
                    }
                    PointSlope slope = new PointSlope(seekerRoid, roidToFind);
                    //Get all whole points on slope
                    List<Point> slopePoints = slope.WholePointsOnSlope();
                    //Get any astroids on this point
                    List<Point> comp = slopePoints.Intersect(Astroids).ToList();
                    //if there's more than one order by distinace 
                    if (comp.Count() > 0)
                    {
                        if (comp.Count() > 1)
                        {
                            comp = comp.OrderBy(x => Dist(slope.StartP, x)).ToList();
                        }
                        //Check if the first visable astroid is the one we're looking for add it to the list if it is
                        //If we know this astroid can't see another then we know that astroid can't see this.
                        //We can also roll that knowlage down the list
                        //But this would need us to set up everything first which I don't think is currently required. 
                        //I could spin each of these off into a Task as well but it only takes a few seconds to run on the input.
                        if (comp.First() == roidToFind)
                        {
                            visableRoids.Add(roidToFind);
                        }
                    }
                }

                roidCount.TryAdd(seekerRoid, visableRoids.Count());
            }



            KeyValuePair<Point, int> bestRoid = roidCount.OrderByDescending(x => x.Value).First();
            Console.WriteLine(String.Format("Best roid {0} with count {1}", bestRoid.Key.ToString(), bestRoid.Value.ToString()));
        }

        static int Dist(Point p1, Point p2)
        {
            Point v = p1 - p2;
            int d = Math.Max(Math.Abs(v.X), Math.Abs(v.Y));
            return d;
        }

        private static void WriteIntersect(List<Point> test, List<Point> points)
        {
            List<Point> comp = test.Intersect(points).ToList();
            foreach (Point p in comp)
            {
                Console.WriteLine(p.ToString());
            }
        }

 
    }
}