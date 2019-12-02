using System;
using System.Collections.Generic;
using System.Text;

namespace Day2
{
    public class IntCodeVM
    {
        int a;
        int b;
        int pCounter;
        bool run;
        public int[] Memory { get; private set; }

        public IntCodeVM(int[] program)
        {
            run = false;
            Clear();
            LoadProgram(program);
        }

        public void LoadProgram(int[] program)
        {
            Memory = (int[])program.Clone();
        }

        public void Execute()
        {
            Clear();
            run = true;
            while (run)
            {
                int code = Memory[pCounter];

                if (code == 1)
                {
                    OppSum();
                }
                else if (code == 2)
                {
                    OppMultiply();
                }
                else if (code == 99)
                {
                    OppExit();
                }
                else
                {
                    Console.WriteLine(String.Format("Error: Unknown OppCode {0}", code));
                    run = false;
                }
            }
        }

        void Clear()
        {
            a = 0;
            b = 0;
            pCounter = 0;
        }

        //Opp codes
        void OppSum()
        {
            ReadA();
            ReadB();
            Sum();
            pCounter += 1;
        }

        void OppMultiply()
        {
            ReadA();
            ReadB();
            Multiply();
            pCounter += 1;
        }

        void OppExit()
        {
            run = false;
        }


        //Instructions 
        void ReadA()
        {
            pCounter += 1;
            a = Memory[Memory[pCounter]];
        }

        void ReadB()
        {
            pCounter += 1;
            b = Memory[Memory[pCounter]];
        }

        void Sum()
        {
            pCounter += 1;
            Memory[Memory[pCounter]] = a + b;
        }

        void Multiply()
        {
            pCounter += 1;
            Memory[Memory[pCounter]] = a * b;
        }
    }
}
