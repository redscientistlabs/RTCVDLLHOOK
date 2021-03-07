using RTCV.CorruptCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XemuVanguardHook
{
    class Shortcuts //I'm calling this class "Shortcuts" because it contains "shortcuts" to other functions
    {
        [DllExport("CPU_STEP")]
        public static void CPU_STEP()
        {
            STEP_CORRUPT();
        }

        private static void STEP_CORRUPT()
        {
            RtcClock.StepCorrupt(true, true);
        }
        [DllExport("InitVanguard")] 
        public static void InitVanguard()
        {
            VanguardCore.Start();
        }
    }
}
