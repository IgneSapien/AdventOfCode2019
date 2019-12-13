using System;
using System.Collections.Generic;
using System.Numerics;

namespace Day12
{
    class Program
    {
        //Should really just parse these.

        //Input
        //<x=-6, y=2, z=-9>
        //<x=12, y=-14, z=-4>
        //<x=9, y=5, z=-6>
        //<x=-1, y=-4, z=9>
        
        //Test1
        //<x=-1, y=0, z=2>
        //<x=2, y=-10, z=-7>
        //<x=4, y=-8, z=8>
        //<x=3, y=5, z=-1>

        static void Main()
        {
            ////input
            Moon[] moons = new Moon[]
            {
                new Moon('I',new Vector3(-6,2,-9)),
                new Moon('E',new Vector3(12,-14,-4)),
                new Moon('G',new Vector3(9,5,-6)),
                new Moon('C',new Vector3(-1,-4,9))
            };

            //test1
            //Moon[] moons = new Moon[]
            //{
            //    new Moon('I',new Vector3(-1,0,2)),
            //    new Moon('E',new Vector3(2,-10,-7)),
            //    new Moon('G',new Vector3(4,-8,8)),
            //    new Moon('C',new Vector3(3,5,-1))
            //};

            //int steps = 1000;
            moons = ProcessOrbit(moons);


            //Part1
            //float enargy = 0;
            //foreach (Moon m in moons)
            //{
            //    enargy += m.GetEnargy();
            //}
            //Console.WriteLine(String.Format("Enargy in system: {0}", enargy.ToString()));

            Console.ReadLine();
        }

        static Moon[] ProcessOrbit(Moon[] moons, double steps = double.MaxValue)
        {
            int mLen = moons.Length;

            //Vector3[] staringP = new Vector3[mLen];

            //for (int m = 0; m < mLen; m++)
            //{
            //    staringP[m] = moons[m].Postion;
            //}

            //Itterate the steps
            for (double i = 0; i < steps; i++)
            {
                
                //Check each moon
                for (int m1 = 0; m1 < mLen; m1++)
                {
                    Moon currentMoon = moons[m1];
                    //Againt every other moon
                    for (int m2 = m1 + 1; m2 < mLen; m2++)
                    {
                        //Seems it'll run the loop first time regardless so need to break if we're at the end.
                        if (m2 >= mLen)
                        {
                            break;
                        }

                        Moon compMoon = moons[m2];
                        ComperMoons(moons[m1], moons[m2]);
                    }
                }

                Console.WriteLine(String.Format("Step: {0}", i.ToString()));
                float enargy = 0;
                foreach (Moon m in moons)
                {
                    m.ApplyVelocity();
                    enargy += m.GetEnargy();
                    //Console.WriteLine(m.ToString());
                }

                //Need do this just on each axis then get the Losest common mulitplier. Again stolen from other people!

                bool match = true;
                for (int m3 = 0; m3 < mLen; m3++)
                {
                    //Can't take credit for this.
                    //Logically you don't have to check postion, we just have to know if the volcity is zero
                    match &= moons[m3].Velocity == Vector3.Zero;
                }

                if(match)
                {
                    //Can't take credit for this either
                    //Planets move in symetrical cycles which means their volocity will reach zero at half the number of steps it'll take to get back to their orignal postion.
                    Console.WriteLine(String.Format("Back to orignal state at step {0}", ((i+1) * 2).ToString()));
                    break;
                }

            }

            return moons;
        }

        static void ComperMoons(Moon moonA, Moon moonB)
        {
            //need three way logic here for ==s
            int vx = 0;
            int vy = 0;
            int vz = 0;
            vx = moonA.Postion.X < moonB.Postion.X ? 1 : -1;
            vx = moonA.Postion.X == moonB.Postion.X ? 0 : vx;

            vy = moonA.Postion.Y < moonB.Postion.Y ? 1 : -1;
            vy = moonA.Postion.Y == moonB.Postion.Y ? 0 : vy;

            vz = moonA.Postion.Z < moonB.Postion.Z ? 1 : -1;
            vz = moonA.Postion.Z == moonB.Postion.Z ? 0 : vz;

            Vector3 va = new Vector3(vx, vy, vz);

            moonA.AddVelocity(va);

            Vector3 vb = new Vector3(vx * -1, vy * -1, vz * -1);

            moonB.AddVelocity(vb);
        }
    }

    class Moon
    {
        public char ID { get; protected set; }
        public Vector3 Postion { get; protected set; }
        public Vector3 Velocity { get; protected set; }

        public Moon(char id, Vector3 startingPos)
        {
            ID = id;
            Postion = startingPos;
            Velocity = Vector3.Zero;
        }

        public void AddVelocity(Vector3 v)
        {
            Velocity = Vector3.Add(Velocity, v);
        }

        public void ApplyVelocity()
        {
            Postion = Vector3.Add(Postion, Velocity);
        }

        public float GetEnargy()
        {
            float p = Math.Abs(Postion.X) + Math.Abs(Postion.Y) + Math.Abs(Postion.Z);
            float v = Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

            return p * v;
        }

        public override string ToString()
        {
            return String.Format("[{6}] pos=<x=-{0}, y=  {1}, z= {2}>, vel=<x= {3}, y= {4}, z= {5}>"
                , Postion.X, Postion.Y, Postion.Z, Velocity.X, Velocity.Y , Velocity.Z, ID.ToString());
        }
    }

}
