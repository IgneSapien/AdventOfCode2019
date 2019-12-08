using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day8
{
    class Program
    {

        static List<int[]> image;

        static void Main()
        {
            
            char[] input = File.ReadAllLines(@"Task1.txt").First().ToCharArray();

            image = new List<int[]>();
            var imageSize = (x: 25, y: 6);
            //var imageSize = (x: 3, y: 2);
            CreateImage(input, imageSize);

            //Part1(imageSize);

            //Part 2
            //Feel like there's a better way of doing this but after being stuck on a stupid bug in part1 I'll take "working"
            int[] outPut = new int[input.Count()];

            for (int y = 0; y < imageSize.y; y++)
            {
                for (int x = 0; x < imageSize.x; x++)
                {
                    int arrayOffSet = x + (y * imageSize.x);
                    foreach (int[] layer in image)
                    {
                        int c = layer[arrayOffSet];
                        if (c != 2)
                        {
                            outPut[arrayOffSet] = c;
                            break;
                        }
                    }
                }
            }            

            for (int y = 0; y < imageSize.y; y++)
            {
                StringBuilder row = new StringBuilder();
                for (int x = 0; x < imageSize.x; x++)
                {
                    int arrayOffSet = x + (y * imageSize.x);
                   
                    if(outPut[arrayOffSet] == 1)
                    {
                        row.Append("#");
                    }
                    else
                    {
                        row.Append(" ");
                    }
                }
                Console.WriteLine(row.ToString());
            }
            Console.WriteLine();
            

            Console.ReadLine();
        }

        private static void Part1((int x, int y) imageSize)
        {
            int[] testLayer2 = image.OrderBy(l => l.Count(p => p == 0)).First();

            for (int y = 0; y < imageSize.y; y++)
            {
                StringBuilder row = new StringBuilder();
                for (int x = 0; x < imageSize.x; x++)
                {
                    int arrayOffSet = x + (y * imageSize.x);
                    row.Append(testLayer2[arrayOffSet].ToString());
                }
                Console.WriteLine(row.ToString());
            }
            Console.WriteLine();
            int checkb0 = testLayer2.Where(i => i == 0).Count();
            int checkb1 = testLayer2.Where(i => i == 1).Count();
            int checkb2 = testLayer2.Where(i => i == 2).Count();
            int checkbSum = checkb1 * checkb2;

            Console.WriteLine(String.Format("0 count: {0}", checkb0.ToString()));
            Console.WriteLine(String.Format("1 count: {0}", checkb1.ToString()));
            Console.WriteLine(String.Format("2 count: {0}", checkb2.ToString()));
            Console.WriteLine(String.Format("1 count * 2 count: {0}", checkbSum.ToString()));
        }

        private static void CreateImage(char[] input, (int x, int y) imageSize)
        {

            int layerLen = (imageSize.x * imageSize.y);
            int layers = input.Length / (layerLen);

            for (int l = 0; l < layers; l++)
            {
                int[] layer = new int[layerLen];
                for (int y = 0; y < imageSize.y; y++)
                {
                    for (int x = 0; x < imageSize.x; x++)
                    {
                        int arrayOffSet = x + (y * imageSize.x);
                        int layerOffSet = l * layerLen;
                        layer[arrayOffSet] = int.Parse(input[arrayOffSet + layerOffSet].ToString());
                    }
                }
                image.Add(layer);
            }
        }
    }
}
