using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Day7
{
    class Amp
    {
        public int Phase { get; set; }
        public IntCodeVM ampVM { get; private set; }
        public Amp LinkedAmp { get; set; }

        public int LastOutput { get; set; }

        public Amp(IntCodeVM vm)
        {
            ampVM = vm;
            this.Phase = 0;
        }

        public void AmpInput(int input)
        {
            ampVM.Input.Add(input);
        }

        public bool AmpOutput(out int r)
        {

            if(ampVM.Output.TryTake(out r))
            {
                LastOutput = r;
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<Task> RunAmp()
        {
            Task vmTask  = Task.Run(() => ampVM.Execute());
            
            while (!vmTask.IsCompleted)
            {
                if (LinkedAmp != null)
                {
                    if (LinkedAmp.ampVM.Output.TryTake(out int r))
                    {
                        
                        ampVM.Input.Add(r);
                    }
                }
            }
            await vmTask;
            return Task.CompletedTask;
        }

    }
}
