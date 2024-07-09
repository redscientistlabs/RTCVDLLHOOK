using RTCV.Common;
using RTCV.CorruptCore;
using RTCV.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DolphinVanguard_Hook
{
    class Shortcuts //I'm calling this class "Shortcuts" because it contains "shortcuts" to other functions
    {
        [DllExport("CORESTEP")]
        public static void CORE_STEP()
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
        [DllExport("GAMETOLOAD")]
        public static void GAMETOLOAD(string rompath)
        {
            VanguardCore.GAME_TO_LOAD = rompath;
        }
        [DllExport("LOADGAMESTART")]
        public static void LOADGAMESTART([MarshalAs(UnmanagedType.BStr)] string rompath)
        {
            VanguardCore.LOAD_GAME_START(rompath);
        }
        [DllExport("LOADGAMEDONE")]
        public static void LOADGAMEDONE([MarshalAs(UnmanagedType.BStr)] string gamename)
        {
            VanguardCore.LOAD_GAME_DONE(gamename);
        }
        [DllExport("GAMECLOSED")]
        public static void GAMECLOSED()
        {
            VanguardCore.GAME_CLOSED();
        }
        [DllExport("EMULATORCLOSING")]
        public static void EMULATORCLOSING()
        {
            VanguardCore.EMULATOR_CLOSING();
        }
        [DllExport("RTCOSDENABLED")]
        public static bool RTCOSDENABLED()
        {
            return VanguardCore.RTC_OSD_ENABLED();
        }
    }
}
