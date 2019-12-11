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
            var input = File.ReadLines(@"./Inputs/Part1.txt");
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
            //Best roid in part one is 11,11 221
            //Best roid in Big test is 11,13 210 
            //Big test 200 = 8,2
            Part2(Astroids, (11, 11));

            Console.ReadLine();
        }


        private static void Part2(List<Point> Astroids, Point BigFingL)
        {
            Astroids.Remove(BigFingL);

            //Since the puzzle input and test have 200+ visable astroid from the stations then we don't need to loop the bellow even if it would be fun

            List<Point> blownUpThisRound;
            Dictionary<double, List<Point>> angleRoids = GetRoidsByAngle(Astroids, BigFingL);

            

            List<(double angle, Point point)> visRoids = new List<(double, Point)>();

            //This time we want to order the the visable roids by the angle so we'll keep the key
            foreach (var kvp in angleRoids)
            {
                visRoids.Add((kvp.Key, kvp.Value.OrderBy(x => Dist(BigFingL, x)).ToList().First()));
            }

            foreach (var r in visRoids)
            {
                Console.WriteLine(String.Format("Orignal, {0}, {1}, {2}", r.point.X.ToString(), r.point.Y.ToString(), r.angle.ToString()));
            }

            visRoids = visRoids.Select(r => (ConvertAngle(r.angle), r.point)).ToList();


            foreach (var r in visRoids)
            {
                Console.WriteLine(String.Format("Adjusted, {0}, {1}, {2}", r.point.X.ToString(), r.point.Y.ToString(), r.angle.ToString()));
            }


            blownUpThisRound = visRoids.OrderBy(r => r.angle).Select(or => or.point).ToList();

            for (int i = 0; i < blownUpThisRound.Count(); i++)
            {
                Console.WriteLine(String.Format("{0} Blowny upy: {1}", (i + 1).ToString(), blownUpThisRound[i].ToString()));
            }


            Console.ReadLine();

        }

        private static double ConvertAngle(double angle)
        {
            //if the angle is negative add 360 to it to get it relative to the counter clockwise posative angle 
            angle = angle < 0 ? 360 + angle : angle;
            //Rotate we're facing "down" and starting from the x 
            //This is horrible but it works on the test input
            angle += 91;
            angle = angle > 360 ? angle - 360 : angle;
            angle -= 1;
            return angle;
        }

        private static void Part1(List<Point> Astroids)
        {
            Dictionary<Point, int> roidCount = new Dictionary<Point, int>();
            foreach (Point seekerRoid in Astroids)
            {
                Dictionary<double, List<Point>> angleRoids = GetRoidsByAngle(Astroids, seekerRoid);

                List<Point> visRoids = GetVisRoids(seekerRoid, angleRoids);

                roidCount.TryAdd(seekerRoid, visRoids.Count());
            }

            KeyValuePair<Point, int> bestRoid = roidCount.OrderByDescending(x => x.Value).First();
            Console.WriteLine(String.Format("Best roid {0} with count {1}", bestRoid.Key.ToString(), bestRoid.Value.ToString()));
        }

        private static List<Point> GetVisRoids(Point seekerRoid, Dictionary<double, List<Point>> angleRoids)
        {
            //Get the first roid from the list per angles; 
            List<Point> visRoids = new List<Point>();
            foreach (var kvp in angleRoids)
            {
                visRoids.Add(kvp.Value.OrderBy(x => Dist(seekerRoid, x)).ToList().First());
            }
            return visRoids;
        }

        private static Dictionary<double, List<Point>> GetRoidsByAngle(List<Point> Astroids, Point seekerRoid)
        {
            //Get vector of all roids relative to seeker
            Dictionary<Point, Point> rebasedRoids = Astroids.ToDictionary(p => p - seekerRoid, p => p);
            Dictionary<double, List<Point>> angleRoids = new Dictionary<double, List<Point>>();
            //Remove the new origin 
            rebasedRoids.Remove((0, 0));

            foreach (var kvp in rebasedRoids)
            {
                double radians = Math.Atan2(kvp.Key.Y, kvp.Key.X);
                double angle = radians * (180 / Math.PI);
                //A positive return value represents a counterclockwise angle from the x-axis; a negative return value represents a clockwise angle.
                if (!angleRoids.TryGetValue(angle, out List<Point> roids))
                {
                    roids = new List<Point>() { kvp.Value };
                    angleRoids.Add(angle, roids);
                }
                else
                {
                    roids.Add(kvp.Value);
                }
            }
            //Group Roids on the same angle 
            return angleRoids;
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