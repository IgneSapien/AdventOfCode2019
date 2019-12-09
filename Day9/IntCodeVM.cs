using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Day9
{
    public class IntCodeVM
    {
        long pCounter;
        long pOffset;

        bool run;

        public bool InputRequired { get; private set; }

        //Working memory, wiped when a program is loaded
        public long[] Memory { get; private set; }

        //A list of programs, currently not required but acts like having the programs saved to a disk.
        //Might want to turn this to a dictoanry 
        List<long[]> programs;

        //Input/Output queues that are thread safe(?)
        BlockingCollection<long> Input;
        public BlockingCollection<long> Output { get; private set; }

        public IntCodeVM(List<long[]> intialPrograms)
        {
            run = false;
            InputRequired = false;
            Input = new BlockingCollection<long>();
            Output = new BlockingCollection<long>();
            programs = intialPrograms;
            //Not sure we should load the program here, I'll often want to run the VM on a loop to try diffrent inputs so it needs loading before running each time.
            //LoadProgram();
        }

        //Whipes memory and loads a program at a given index of the program list into the start of the memeory array 
        //TODO: Need to handle trying to load programs that don't exist. 
        public void LoadProgram(int programNumber = 0)
        {
            //Arrays can only be a max of 2GB unless we're x64 and set <gcAllowVeryLargeObjects enabled="true" /> 
            //Not sure how much memeory we actually need but it's possible we'll get out of range exceptions 
            //5000 is more than enough for the dignotistic program
            //Not sure if it's effcent to crate a new array everytime but should be safe.
            Memory = new long[50000000];
            long[] program = programs[programNumber];
            Array.Copy(program, Memory, program.Length);
        }

        public void AddProgram(long[] program)
        {
            programs.Add(program);
        }

        public void AddInput(long input)
        {
            InputRequired = false;
            Input.Add(input);
        }


        //Run the program
        //Runs as an async task so the program can wait on any required inputs with out blocking.
        public Task Execute()
        {
            pCounter = 0;
            pOffset = 0;
            run = true;
            while (run)
            {
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
                    case "03": OppInput(param); break;
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

        //01
        void OppSum(string param)
        {
            long a = Read(param[2]);
            long b = Read(param[1]);
            Write(param[0], a + b);
            pCounter += 1;
        }

        //02
        void OppMultiply(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppMultiply with params: {0}", param));
            long a = Read(param[2]);
            long b = Read(param[1]);
            Write(param[0], a * b);
            pCounter += 1;
        }

        //03
        void OppInput(string param)
        {
            //Console.WriteLine("Excuting OppInput");
            Write(param[2], ReadIn());
            pCounter += 1;
        }

        //04
        void OppOutput(string param)
        {
            //Console.WriteLine("Excuting OppOutput");
            Output.Add(Read(param[2]));
            pCounter += 1;
        }

        //05
        void OppJumpTrue(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppJumpTrue with params: {0}", param));
            long a = Read(param[2]);
            long b = Read(param[1]);

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
            long a = Read(param[2]);
            long b = Read(param[1]);

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
            long a = Read(param[2]);
            long b = Read(param[1]);
            Write(param[0], a < b ? 1 : 0);
            pCounter += 1;
        }

        //08
        void OppEquals(string param)
        {
            //Console.WriteLine(String.Format("Excuting OppEquals with params: {0}", param));
            long a = Read(param[2]);
            long b = Read(param[1]);
            Write(param[0], a == b ? 1 : 0);
            pCounter += 1;
        }

        //09
        //Adjust the current offset by the paramter 
        void OppOffset(string param)
        {
            //Console.WriteLine(String.Format("Adjusting offset with params: {0}", param));
            long a = Read(param[2]);
            pOffset += a;
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
        long ReadIn()
        {
            long r;
            while (!Input.TryTake(out r))
            {
                //If we can't grab the input from the queue we'll wait
                //We'll flag input as being needed to make working with the VM from outside easier 
                InputRequired = true;
                //Don't want it to keep setting InputRequired but I don't know if this is the best way of doing it.
                Thread.Sleep(50);
            }
            InputRequired = false;
            return r;
        }


        //Read/Write Memory 

        //Read
        //This instuctions has to work in versious modes so it switchs on them.
        
        long Read(char mode)
        {
            pCounter += 1;
            return mode switch
            {
                '0' => ReadPosition(),
                '1' => ReadImmediate(),
                '2' => ReadOffset(),
                _ => throw new ArgumentException(message: "invalid Read Mode value"),
            };
        }

        long ReadPosition()
        {
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {2}", pCounter, Memory[Memory[pCounter]], Memory[pCounter]));
            return Memory[Memory[pCounter]];
        }

       
        long ReadImmediate()
        {
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {0}", pCounter, Memory[pCounter]));
            return Memory[pCounter];
        }

        //Could do this as the ReadPostionToReg with an optional paramater but I like this being clear.
        long ReadOffset()
        {
            //Console.WriteLine(String.Format("Step {0}: Reading {1} from address {2}", pCounter, Memory[Memory[pCounter]], Memory[pCounter]));
            long offset = Memory[pCounter] + pOffset;
            return Memory[offset];
        }

        //Write to Memoery 
        //This instuctions has to work in versious modes so it switchs on them.
        void Write(char mode, long val)
        {
            pCounter += 1;
            switch (mode)
            {
                case '0': WriteToPosition(val); break;
                //case '1': WriteToImmediate(val); break;
                case '2': WriteToOffsetPosition(val); break;
                default:
                    Console.WriteLine(String.Format("Error: Unknown Write Mode {0}", mode.ToString()));
                    run = false;
                    break;
            };
        }

        void WriteToPosition(long val)
        {
            //Console.WriteLine(String.Format("Step {0}: Writing {1} to memory to address {2}", pCounter, bus, Memory[pCounter]));
            Memory[Memory[pCounter]] = val;
        }

        //This shouldn't be used? 
        //void WriteToImmediate(long val)
        //{
        //    //Console.WriteLine(String.Format("Step {0}: Writing {1} to address {0}", pCounter, bus));
        //    Memory[pCounter] = val;
        //}

        void WriteToOffsetPosition(long val)
        {
            long offset = Memory[pCounter] + pOffset;
            //Console.WriteLine(String.Format("Step {0}: Writing {1} to memory to address {2}", pCounter, bus, Memory[pCounter]));
            Memory[offset] = val;
        }
    }
}
