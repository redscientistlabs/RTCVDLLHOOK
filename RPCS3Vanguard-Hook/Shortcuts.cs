using RTCV.CorruptCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCS3Vanguard_Hook
{
    class Shortcuts //I'm calling this class "Shortcuts" because it contains "shortcuts" to other functions
    {
        [DllExport("CPU_STEP")]
        public static void CPU_STEP()
        {
            STEP_CORRUPT();
        }

        public static void STEP_CORRUPT()
        {
            StepActions.Execute();
            RtcClock.StepCorrupt(true, true);
        }
        [DllExport("InitVanguard")] 
        public static void InitVanguard()
        {
            VanguardCore.Start();
        }
        [DllExport("LOADGAMEDONE")]
        public static void LOADGAMEDONE()
        {
            VanguardCore.LOAD_GAME_DONE();
        }
    }
}
