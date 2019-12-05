using System;
using System.Collections.Generic;
using System.Text;

namespace Day5
{
    public class IntCodeVM
    {
        int a;
        int b;
        int bus;
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
            pCounter = 0;
            run = true;
            while (run)
            {
                //just to be on the safe side
                Clear();
                string code = Memory[pCounter].ToString("00000");
                string opp = code.Substring(3, 2);
                string param = code.Substring(0, 3);

                //Console.WriteLine(String.Format("Step {0}: Excuting {1}", pCounter, code));

                switch (opp)
                {
                    case "01": OppSum(param); break;
                    case "02": OppMultiply(param); break;
                    case "03": OppInput(); break;
                    case "04": OppOutput(param); break;
                    case "05": OppJumpTrue(param); break;
                    case "06": OppJumpFalse(param); break;
                    case "07": OppLessThan(param); break;
                    case "08": OppEquals(param); break;
                    case "99": OppExit(); break;
                    default:
                        Console.WriteLine(String.Format("Error: Unknown OppCode {0}", opp));
                        run = false;
                    break;
                };


            }
        }

        void Clear()
        {
            a = 0;
            b = 0;
            bus = 0;
        }

        //** Opp codes **//
        //TODO: Proper logging 
        //TODO: Can extract some common steps

        //01
        void OppSum(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppSum with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            //Console.WriteLine("Writing to B");
            ReadToReg(param[1], ref b);
            Sum();
            Write(param[0]);
            pCounter += 1;
        }

        //02
        void OppMultiply(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppMultiply with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            //Console.WriteLine("Writing to B");
            ReadToReg(param[1], ref b);
            Multiply();
            Write(param[0]);
            pCounter += 1;
        }

        //03
        void OppInput()
        {
            //Console.WriteLine("Excuting OppInput");
            ReadIn();
            Write('0');
            pCounter += 1;
        }

        //04
        void OppOutput(string param)
        {
            //Console.WriteLine("Excuting OppOutput");
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            WriteOut();
            pCounter += 1;
        }

        //05
        void OppJumpTrue(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppJumpTrue with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            //Console.WriteLine("Writing to B");
            ReadToReg(param[1], ref b);
            //Shame I can't think of a way to do this with instructions 
            //Console.WriteLine(String.Format("Does a == 0?: {0}", a));
            if (a == 0)
            {
                //Console.WriteLine("Yes, doing nothing");
                pCounter += 1; 
            } else
            {
                //Console.WriteLine(String.Format("No jumping to {0}", b));
                pCounter = b;
            }
        }

        //06
        void OppJumpFalse(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppJumpFalse with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            //Console.WriteLine("Writing to B");
            ReadToReg(param[1], ref b);
            //Shame I can't think of a way to do this with instructions 
            //Console.WriteLine(String.Format("Does a == 0?: {0}", a));
            if (a == 0)
            {
                //Console.WriteLine(String.Format("Yes jumping to {0}", b));
                pCounter = b;
            }
            else
            {
                //Console.WriteLine("No, doing nothing");
                pCounter += 1;
            }
            return;
        }

        //07
        void OppLessThan(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppLessThan with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            //Console.WriteLine("Writing to B");
            ReadToReg(param[1], ref b);
            LessThan();
            Write(param[0]);
            pCounter += 1;
        }

        //08
        void OppEquals(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppEquals with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            //Console.WriteLine("Writing to B");
            ReadToReg(param[1], ref b);
            Equals();
            Write(param[0]);
            pCounter += 1;
        }

        //99
        void OppExit()
        {
            //Console.WriteLine("Excuting OppExit");
            run = false;
        }

        //** Instructions **//

        //IO
        void ReadIn()
        {
            Console.WriteLine("Enter int");
            //TODO: This should only really read ints
            bus = Int32.Parse(Console.ReadLine());
        }

        void WriteOut()
        {
            Console.WriteLine(String.Format("Program output at step {0}: {1}", pCounter, a));
        }


        //Read To Register 
        void ReadToReg(char mode, ref int reg)
        {
            if (mode == '0')
            {
                ReadPositionToReg(ref reg);
            }
            else
            {
                ReadImmediateToReg(ref reg);
            }
        }

        void ReadPositionToReg(ref int reg)
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {2}", pCounter, Memory[Memory[pCounter]], Memory[pCounter]));
            reg = Memory[Memory[pCounter]];
        }

        void ReadImmediateToReg(ref int reg)
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {0}", pCounter, Memory[pCounter]));
            reg = Memory[pCounter];
        }



        //Opperations 
        void Sum()
        {
            //Console.WriteLine(String.Format("Sum A + B: {0} + {1} = {2}",a,b,a + b));
            bus = a + b;
        }

        void Multiply()
        {
            //Console.WriteLine(String.Format("Multiply A * B: {0} * {1} = {2}", a, b, a * b));
            bus = a * b;
        }

        void LessThan()
        {
            //Console.WriteLine(String.Format("Equals A < B: {0} == {1} = {2}", a, b, (a < b).ToString()));
            bus = a < b ? 1 : 0;
        }

        void Equals()
        {
            //Console.WriteLine(String.Format("Equals A = B: {0} == {1} = {2}", a, b, (a == b).ToString()));
            bus = a == b ? 1 : 0;
        }

        //Write to Memoery 
        void Write(char mode)
        {
            if (mode == '0')
            {
                WriteToPosition();
            }
            else
            {
                WriteToImmediate();
            }

        }

        void WriteToPosition()
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Writing {1} to memory to address {2}", pCounter, bus, Memory[pCounter]));
            Memory[Memory[pCounter]] = bus;
        }


        void WriteToImmediate()
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Writing {1} to address {0}", pCounter, bus));
            Memory[pCounter] = bus;
        }


    }
}
