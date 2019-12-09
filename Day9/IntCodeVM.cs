using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Day9
{
    //public enum OppCodes { Sum, Multiply, Input, Output, JumpTrue, JumpFalse, LessThan, Equals, Exit = 99 }
    //public enum Modes { Position, Immediate, Relative }


    public class IntCodeVM
    {
        //Thought it would be fun to treat the VM a bit more like a actual computer with regeistesr etc.
        long a;
        long b;
        long bus;
        long pCounter;
        long pOffset;
        bool run;

        //Working memory, wiped when a program is loaded
        public long[] Memory { get; private set; }

        //A list of programs, currently not required but acts like having the programs saved to a disk.
        List<long[]> programs;

        //Input/Output queues that are thread safe(?)
        public BlockingCollection<long> Input { get; private set; }
        public BlockingCollection<long> Output { get; private set; }

        public IntCodeVM(long[] defaultProgram)
        {
            run = false;
            Input = new BlockingCollection<long>();
            Output = new BlockingCollection<long>();
            Clear();
            programs = new List<long[]>
            {
                defaultProgram
            };
            //Not sure we should load the program here, I'll often want to run the VM on a loop to try diffrent inputs so it needs loading before running each time.
            //LoadProgram();
        }

        //Whipes memory and loads a program at a given index of the program list into the start of the memeory array 
        public void LoadProgram(int programNumber = 0)
        {
            //Arrays can only be a max of 2GB unless we're x64 and set <gcAllowVeryLargeObjects enabled="true" /> 
            //Not sure how much memeory we actually need but it's possible we'll get out of range exceptions 
            //5000 is more than enough for the dignotistic program
            //Not sure if it's effcent to crate a new array everytime but should be safe.
            Memory = new long[10000];
            long[] program = programs[programNumber];
            Array.Copy(program, Memory, program.Length);
        }


        //Run the program
        //Runs as an async task so the program can wait on any required inputs with out blocking.
        public Task Execute()
        {
            Clear();
            pCounter = 0;
            pOffset = 0;
            run = true;
            while (run)
            {
                //just to be on the safe side
                Clear();
                //Instructions are encoded [param3][param2][param1][[oppcode]] but with leading zeros removed
                //This puts the zeros in so we can treat all instructions the same way
                string code = Memory[pCounter].ToString("00000");
                //Splits out the instruction into params/OppCode
                //I thought enums would make things more readable then I remembered that emums are annoying
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
                    case "09": OppOffset(param); break;
                    case "99": OppExit(); break;
                    default:
                        Console.WriteLine(String.Format("Error: Unknown OppCode {0}", opp));
                        run = false;
                        break;
                };
            }
            return Task.CompletedTask;
        }

        //Clears up the regiester and bus, shouldn't stricntly be nessicary but will help with debugging 
        void Clear()
        {
            a = 0;
            b = 0;
            bus = 0;
        }

        //Clear down the input/output queue if you want to start a program fresh.
        public void ClearInputOutput()
        {
            while (Input.TryTake(out _))
            {
            }
            while (Output.TryTake(out _))
            {
            }
        }


        //** Opp codes **//
        //TODO: Proper logging 

        //Opp codes are made up of indiviudal instructions for the fun of it.

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
            WriteAOut();
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
            }
            else
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

        //09
        //Adjust the current offset by the paramter 
        void OppOffset(string param)
        {
            //Console.WriteLine(String.Format("Adjusting offset with params: {0}", param));
            //Console.WriteLine("Writing to A");
            ReadToReg(param[2], ref a);
            AddAToOffset();
            pCounter += 1;
        }

        //99
        void OppExit()
        {
            //Console.WriteLine("Excuting OppExit");
            run = false;
        }

        //** Instructions **//
        //The instuctions that make up the OppCodes

        //IO
        void ReadIn()
        {
            while (!Input.TryTake(out bus))
            {
                //should grab the first item from the queu then break? 
            }
        }

        void WriteAOut()
        {
            //Console.WriteLine(String.Format("Program output at step {0}: {1}", pCounter, a));
            Output.Add(a);
        }


        //Read/Write Memory 

        //Read To Register 
        //This instuctions has to work in versious modes so it switchs on them.
        
        void ReadToReg(char mode, ref long reg)
        {
            switch (mode)
            {
                case '0': ReadPositionToReg(ref reg); break;
                case '1': ReadImmediateToReg(ref reg); break;
                case '2': ReadOffsetPositionToReg(ref reg); break;
                default:
                    Console.WriteLine(String.Format("Error: Unknown Read Mode {0}", mode.ToString()));
                    run = false;
                    break;
            };
        }

        void ReadPositionToReg(ref long reg)
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {2}", pCounter, Memory[Memory[pCounter]], Memory[pCounter]));
            reg = Memory[Memory[pCounter]];
        }

       
        void ReadImmediateToReg(ref long reg)
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {0}", pCounter, Memory[pCounter]));
            reg = Memory[pCounter];
        }

        //Could do this as the ReadPostionToReg with an optional paramater but I like this being clear.
        void ReadOffsetPositionToReg(ref long reg)
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {2}", pCounter, Memory[Memory[pCounter]], Memory[pCounter]));
            reg = Memory[Memory[pCounter + pOffset]];
        }

        //Write to Memoery 
        //This instuctions has to work in versious modes so it switchs on them.
        void Write(char mode)
        {
            switch (mode)
            {
                case '0': WriteToPosition(); break;
                case '1': WriteToImmediate(); break;
                case '2': WriteToOffsetPosition(); break;
                default:
                    Console.WriteLine(String.Format("Error: Unknown Write Mode {0}", mode.ToString()));
                    run = false;
                    break;
            };
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

        void WriteToOffsetPosition()
        {
            pCounter += 1;
            //Console.WriteLine(String.Format("Step {0}: Writing {1} to memory to address {2}", pCounter, bus, Memory[pCounter]));
            Memory[Memory[pCounter + pOffset]] = bus;
        }


        //Pointer intruscitons 
        void AddAToOffset()
        {
            pOffset += a;
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




    }
}
