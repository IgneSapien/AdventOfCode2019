﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using System.Linq;
using System.Threading;


namespace Day15
{
    //TODO: Robot seems to be wandering around the maze but it's not directed and it's not building a path
    //Should make it do a BFS.

    public class Robot
    {
        IntCodeVM Brain;
        Task vmTask;

        public Point Location { get; private set; }
        Direction currentDir;

        Point oxGen = Point.None;

        Dictionary<Point, long> map;

        List<Point> path;

        bool run = false;
        public Robot(List<long[]> intialPrograms)
        {
            Location = (0,0);
            currentDir = Direction.Up;
            map = new Dictionary<Point, long>
            {
                { Location, 1 }
            };
            path = new List<Point>
            {
                Location
            };
            Brain = new IntCodeVM(intialPrograms);
            Brain.LoadProgram();
        }

        public void Start()
        {
            //Part 1 during the full map
            run = true;
            vmTask = Task.Run(() => Brain.Execute());
            while (!vmTask.IsCompleted && run)
            {
                AttemptMove();
                ProcessMove();
            }
            DrawMap(false);

            //Part2 flood the map 
            int m = 0;
            while(true)
            {
                m++;
                Console.WriteLine("Minutes: {0}", m);
                //Get tiles with oxegan in them
                List<Point> oxTiles = map.Where(p => p.Value == 2).Select(p => p.Key).ToList();

                foreach(Point p in oxTiles)
                {
                    List<Point> n = new List<Point>();

                    n.Add(p + Direction.Up);
                    n.Add(p + Direction.Down);
                    n.Add(p + Direction.Left);
                    n.Add(p + Direction.Right);

                    foreach(Point np in n)
                    {
                        if(map.ContainsKey(np))
                        {
                            if(map[np] == 1)
                            {
                                map[np] = 2;
                            }
                        }
                    }
                }

                DrawMap(false);

                List<Point> floorTiles = map.Where(p => p.Value == 1).Select(p => p.Key).ToList();
                if(floorTiles.Count == 0)
                {
                    Console.WriteLine("All floor tiles filled");
                    break;
                }
            }

        }


        private  void ProcessMove()
        {
            long outPut;
            while (!Brain.ReadFromOutput.TryRead(out outPut))
            { }
           
            if (outPut == 0)
            {
                Point p = Location + currentDir;
                if (!map.ContainsKey(p))
                {
                    map.Add(p, 0);
                }
            }
            else if (outPut > 0)
            {
                Location += currentDir;
                if(path.Last() != Location)
                {

                   path.Add(Location);

                }

                if (!map.ContainsKey(Location))
                {
                    map.Add(Location, outPut);
                }

                if(outPut == 2)
                {
                    Console.WriteLine("Found ox gen!");
                    oxGen = Location;
                    DrawMap();
                }
            }
            currentDir = NewDirection();
        }

        Direction NewDirection()
        {
            List<Point> n = new List<Point>();
            Direction d = currentDir;
            n.Add(Location + d);
            for (int i = 0; i < 3; i++)
            {
                d -= 2;
                n.Add(Location + d);
            }


            foreach (Point p in n)
            {
                if(!map.ContainsKey(p))
                {
                    //Unexplored tile found, head in that direction 
                    return Direction.GetCardinalDirection(p - Location);
                }
            }

            //If we get here all tiles around are explored so lets head back up the path.
            path.RemoveAt(path.Count - 1);
            if(path.Count() == 0)
            {
                run = false;
                Console.WriteLine("Explored whole map, returned to start");
                return currentDir;
            }
            Point bt = path.Last();
            Direction r = Direction.GetCardinalDirection(bt - Location);
            return r;
        }

        void AttemptMove()
        {
            switch (currentDir.Type)
            {
                case Direction.Types.Up: Brain.WriteToInput.TryWrite(1); break;
                case Direction.Types.Down: Brain.WriteToInput.TryWrite(2); break;
                case Direction.Types.Left: Brain.WriteToInput.TryWrite(3); break;
                case Direction.Types.Right: Brain.WriteToInput.TryWrite(4); break;
                default:
                    Console.WriteLine("Unknonw direction");
                    break;
            };
        }


        void DrawMap(bool showBot = true)
        {
            //Console.Clear();
            //I was off by one! The path containts the starting point which isn't a "move" 
            if (path.Count > 1)
            {
                Console.WriteLine("Part 1: Number of moves required {0}", path.Count() - 1);
            }
            Console.WriteLine("[Drawing map]");
            
            int minX = map.Keys.Select(x => x.X).Min();
            int maxX = map.Keys.Select(x => x.X).Max();

            int maxY = map.Keys.Select(y => y.Y).Max();
            int minY = map.Keys.Select(y => y.Y).Min();

            //int width = Math.Abs(minY) + Math.Abs(maxX);
            //int height = Math.Abs(minY) + Math.Abs(maxY);

            for (int y = minY; y <= maxY; y++)
            {
                StringBuilder sb = new StringBuilder();
                for (int x = minX; x <= maxX ; x++)
                {
                    if (Location == (x, y) && showBot)
                    {
                        sb.Append("D");
                    }
                    else
                    {
                        if (map.TryGetValue((x,y), out long output))
                        {
                            if (path.Contains((x, y)) && showBot)
                            {
                                //Adds a coloured path red\.\reset
                                sb.Append("\u001b[41m.\u001b[0m");
                            }
                            else
                            {
                                if (showBot)
                                {
                                    sb.Append(output == 0 ? "#" : output == 1 ? "." : "X");
                                }
                                else
                                {
                                    switch (output)
                                    {
                                        case 0: sb.Append("#"); break;
                                        case 1: sb.Append("."); break;
                                        case 2: sb.Append("O"); break;
                                        default:
                                            Console.WriteLine("Unknonw tile");
                                            break;
                                    };
                                }
                            }
                        }
                        else
                        {
                            //Assume if we're not showing the bot we show unknow tiles as walls
                            sb.Append(showBot ? " " : "#");
                        }
                    }
                }
                Console.WriteLine(sb.ToString());
            }

            //TODO: vector againts the min so we can start from 0,0?
        }
    }
}
