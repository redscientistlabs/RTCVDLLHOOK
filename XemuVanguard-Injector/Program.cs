using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHook;

namespace XemuVanguard_Injector
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 targetPID = 0;
            string TargetExe = null;
            string channelName = null;
            EasyHook.RemoteHooking.IpcCreateServer<XemuVanguardHook.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);
            string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "XemuVanguard-Hook.dll");
        }
    }
}
