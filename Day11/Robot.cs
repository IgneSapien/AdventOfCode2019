using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using System.Linq;
using System.Threading;

namespace Day11
{
    public class Robot
    {
        public enum Modes { Scan, Paint, Move }

        IntCodeVM Brain;
        Task vmTask;

        public Modes Mode { get; private set; }
        public Point Location { get; private set; }
        public Direction Facing { get; private set; }

        int i;
        

        Dictionary<Point, long> map;

        public Robot(Point startingPoint, List<long[]> intialPrograms)
        {
            Location = startingPoint;
            map = new Dictionary<Point, long>
            {
                //Start on white.
                { Location, 1 }
            };
            Facing = Direction.Up;
            Mode = Modes.Scan;
            Brain = new IntCodeVM(intialPrograms);
            Brain.LoadProgram();
        }

        public void StartPainting()
        {
            i = 0;
            vmTask = Task.Run(() => Brain.Execute());
            while (!vmTask.IsCompleted)
            {
                switch (Mode)
                {
                    case Modes.Scan: Scan(); break;
                    case Modes.Paint: Paint(); break;
                    case Modes.Move: Move(); break;
                    default:
                        Console.WriteLine(String.Format("Error: Unknown mode {0}", Mode.ToString()));
                        break;
                }

                i++;

                Console.WriteLine("[{0} {1}] {2} tiles painted", i.ToString("00000"), Location.ToString(), map.Count().ToString());
                DrawMap();
            }
            DrawMap();
            Console.WriteLine("Finsihed");
        }

        void Scan()
        {
            if (!map.TryGetValue(Location, out long input))
            {
                //default to black
                input = 0;
                map.Add(Location, input);
            }

            Brain.AddInput(input);
            Mode = Modes.Paint;
            //What to see if we've got any wait condiation issues
            //Turns out we do. Will have to look into that this delay is enough 
            Thread.Sleep(10);
        }

        void Paint()
        {
            long outPut;
            while (!Brain.Output.TryTake(out outPut))
            {
                //wait for output if there is none
            }

            map[Location] = outPut;
            Mode = Modes.Move;
        }

        void Move()
        {
            long outPut;
            while (!Brain.Output.TryTake(out outPut))
            {
                //Waitng for output
            }

            if(outPut == 0)
            {
                //Think this is right, will soon find out.
                Facing -= 2;
            }
            else
            {
                Facing += 2;
            }

            //Thanks to the primvatives I can just add the facing to my location to move.
            Location += Facing;

            Mode = Modes.Scan;
        }

        void DrawMap()
        {
            //Console.Clear();
            Console.WriteLine("Drawing map...");
            
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
                    if (Location == (x, y))
                    {
                        //need to work out why the switch wans't working, I think I'm grabing the enum from the wrong place
                        if (Facing == Direction.Up)
                        {
                            sb.Append("^");
                        }
                        else if (Facing == Direction.Down)
                        {
                            sb.Append("V");
                        }
                        else if (Facing == Direction.Left)
                        {
                            sb.Append("<");
                        }
                        else if (Facing == Direction.Right)
                        {
                            sb.Append(">");
                        }
                    }
                    else
                    {
                        if (map.TryGetValue((x,y), out long output))
                        {
                            sb.Append(output == 0 ? " " : "#");
                        }
                        else
                        {
                            sb.Append("#");
                        }
                    }
                }
                Console.WriteLine(sb.ToString());
            }

            //TODO: vector againts the min so we can start from 0,0?
        }


    }
}
