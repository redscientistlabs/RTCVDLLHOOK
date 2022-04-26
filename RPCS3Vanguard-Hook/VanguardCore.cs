
using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.NetCore.Commands;
using RTCV.Vanguard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Timer = System.Threading.Timer;
using System.Linq;
using System.Windows.Threading;
//using System.Windows.Threading;

namespace RPCS3Vanguard_Hook
{
    class VanguardCore
	{
		
		public static string[] args;
        public static System.Timers.Timer focusTimer;
        public static bool FirstConnect = true;
		public static Form SyncForm;
        public static VanguardRealTimeEvents RTE_API = new VanguardRealTimeEvents();
        public static bool attached = false;
		public static Process xemu;
		public static string System
		{
			get => (string)AllSpec.VanguardSpec[VSPEC.SYSTEM];
			set => AllSpec.VanguardSpec.Update(VSPEC.SYSTEM, value);
		}
		public static string GameName
		{
			get => (string)AllSpec.VanguardSpec[VSPEC.GAMENAME];
			set => AllSpec.VanguardSpec.Update(VSPEC.GAMENAME, value);
		}
		public static string SystemPrefix
		{
			get => (string)AllSpec.VanguardSpec[VSPEC.SYSTEMPREFIX];
			set => AllSpec.VanguardSpec.Update(VSPEC.SYSTEMPREFIX, value);
		}
		public static string SystemCore
		{
			get => (string)AllSpec.VanguardSpec[VSPEC.SYSTEMCORE];
			set => AllSpec.VanguardSpec.Update(VSPEC.SYSTEMCORE, value);
		}
		public static string SyncSettings
		{
			get => (string)AllSpec.VanguardSpec[VSPEC.SYNCSETTINGS];
			set => AllSpec.VanguardSpec.Update(VSPEC.SYNCSETTINGS, value);
		}
		public static string OpenRomFilename
		{
			get => (string)AllSpec.VanguardSpec[VSPEC.OPENROMFILENAME];
			set => AllSpec.VanguardSpec.Update(VSPEC.OPENROMFILENAME, value);
		}
		public static int LastLoadedRom
		{
			get => (int)AllSpec.VanguardSpec[VSPEC.CORE_LASTLOADERROM];
			set => AllSpec.VanguardSpec.Update(VSPEC.CORE_LASTLOADERROM, value);
		}
		public static string[] BlacklistedDomains
		{
			get => (string[])AllSpec.VanguardSpec[VSPEC.MEMORYDOMAINS_BLACKLISTEDDOMAINS];
			set => AllSpec.VanguardSpec.Update(VSPEC.MEMORYDOMAINS_BLACKLISTEDDOMAINS, value);
		}
		public static MemoryDomainProxy[] MemoryInterfaces
		{
			get => (MemoryDomainProxy[])AllSpec.VanguardSpec[VSPEC.MEMORYDOMAINS_INTERFACES];
			set => AllSpec.VanguardSpec.Update(VSPEC.MEMORYDOMAINS_INTERFACES, value);
		}
		public static string emuDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public static string logPath = Path.Combine(emuDir, "EMU_LOG.txt");
		public static string RTCVHookOGLVersion = "0.0.1";
		
        public static PartialSpec getDefaultPartial()
        {
			var partial = new PartialSpec("VanguardSpec");
			partial[VSPEC.NAME] = "RPCS3";
			partial[VSPEC.SYSTEM] = "PS3";
			partial[VSPEC.GAMENAME] = "IGNORE";
			partial[VSPEC.SYSTEMPREFIX] = "PS3";
			partial[VSPEC.OPENROMFILENAME] = "IGNORE";
			partial[VSPEC.SYNCSETTINGS] = String.Empty;
			partial[VSPEC.OVERRIDE_DEFAULTMAXINTENSITY] = 1000000;
			partial[VSPEC.MEMORYDOMAINS_BLACKLISTEDDOMAINS] = new string[] { };
			partial[VSPEC.MEMORYDOMAINS_INTERFACES] = new MemoryDomainProxy[] { };
			partial[VSPEC.CORE_LASTLOADERROM] = -1;
			partial[VSPEC.SUPPORTS_RENDERING] = false;
			partial[VSPEC.SUPPORTS_CONFIG_MANAGEMENT] = true;
			partial[VSPEC.SUPPORTS_CONFIG_HANDOFF] = true;
			partial[VSPEC.SUPPORTS_KILLSWITCH] = true;
			partial[VSPEC.SUPPORTS_REALTIME] = true;
			partial[VSPEC.SUPPORTS_SAVESTATES] = true;
			partial[VSPEC.SUPPORTS_REFERENCES] = true;
			partial[VSPEC.CORE_DISKBASED] = false;
			partial[VSPEC.SUPPORTS_MIXED_STOCKPILE] = true;
			partial[VSPEC.CONFIG_PATHS] = new[] { "" };
			partial[VSPEC.EMUDIR] = emuDir;
			return partial;
        }
		internal static void CreateVmdText(string domain, string text)
        {
			LocalNetCoreRouter.Route(Endpoints.UI, Remote.GenerateVMDText, new object[] { domain, text }, false);
        }
		public static void RegisterVanguardSpec()
        {
			PartialSpec emuSpecTemplate = new PartialSpec("VanguardSpec");
			emuSpecTemplate.Insert(VanguardCore.getDefaultPartial());
			AllSpec.VanguardSpec = new FullSpec(emuSpecTemplate, !RtcCore.Attached);

			if (VanguardCore.attached)
				VanguardConnector.PushVanguardSpecRef(AllSpec.VanguardSpec);
			LocalNetCoreRouter.Route(Endpoints.CorruptCore, Remote.PushVanguardSpec, emuSpecTemplate, true);
			LocalNetCoreRouter.Route(Endpoints.UI, Remote.PushVanguardSpec, emuSpecTemplate, true);

			AllSpec.VanguardSpec.SpecUpdated += new EventHandler<SpecUpdateEventArgs>(OnVanguardSpecOnSpecUpdated);
        }
		private static void OnVanguardSpecOnSpecUpdated(object o, SpecUpdateEventArgs e)
		{
			PartialSpec partial = e.partialSpec;


			LocalNetCoreRouter.Route(RTCV.NetCore.Endpoints.CorruptCore, RTCV.NetCore.Commands.Remote.PushVanguardSpecUpdate, partial, true);
			LocalNetCoreRouter.Route(RTCV.NetCore.Endpoints.UI, RTCV.NetCore.Commands.Remote.PushVanguardSpecUpdate, partial, true);
		}
		public static void EmuThreadExecute(Action callback)
        {
			Dispatcher.CurrentDispatcher.Invoke((MethodInvoker)delegate
			{
				callback();
			}, null);

		}
		public static void LOAD_GAME_START(string rompath)
        {
			StepActions.ClearStepBlastUnits();
			RtcClock.ResetCount();
			VanguardImplementation.LoadROM(rompath);
			LOAD_GAME_DONE();
        }
		public static void LOAD_GAME_DONE()
		{
			PartialSpec gameDone = new PartialSpec("VanguardSpec");
			VanguardImplementation.RefreshDomains();
			gameDone[VSPEC.GAMENAME] = VanguardImplementation.GetGameName();
			//gameDone[VSPEC.OPENROMFILENAME] = VanguardImplementation.GetDVD();
			AllSpec.VanguardSpec.Update(gameDone);
			RtcCore.InvokeLoadGameDone();
		}
		public static void Start()
		{
			//SyncForm = new AnchorForm();
			//         //Grab an object on the main thread to use for netcore invokes
			//         Dispatcher.CurrentDispatcher.Invoke((MethodInvoker)delegate
			//         {
			//             SyncForm.Activate();

			//         }, null);
			//         //IntPtr Handle = SyncForm.Handle;
			//SyncObjectSingleton.SyncObject = SyncForm;
			SyncForm = new AnchorForm();
			var handle = SyncForm.Handle;
			SyncObjectSingleton.SyncObject = SyncForm;

			//SyncObjectSingleton.EmuInvokeDelegate = new SyncObjectSingleton.ActionDelegate(EmuThreadExecute);
			SyncObjectSingleton.EmuThreadIsMainThread = true;
			SyncForm.Show();
			SyncForm.Activate();
			//Start everything
			VanguardImplementation.StartClient();
			VanguardCore.RegisterVanguardSpec();
			RtcCore.StartEmuSide();
			Thread.Sleep(500);
			focusTimer = new System.Timers.Timer
			{
				AutoReset = true,
				Interval = 250
			};
			//Update the focus state of the emulator
			focusTimer.Elapsed += (sender, eventArgs) =>
			{
				if (VanguardCore.attached)
				{
					focusTimer.Enabled = false;
					return;
				}
				if (VanguardImplementation.connector.netConn.status == RTCV.NetCore.Enums.NetworkStatus.CONNECTED)
				{
					var state = Form.ActiveForm != null;
					//Console.WriteLine(state);
					//Shortcuts.STEP_CORRUPT();
					if (((bool?)RTCV.NetCore.AllSpec.VanguardSpec?[RTCV.NetCore.Commands.Emulator.InFocus] ?? true) != state)
						RTCV.NetCore.AllSpec.VanguardSpec?.Update(RTCV.NetCore.Commands.Emulator.InFocus, state, true, false);
				}
			};
			focusTimer.Start();


			//Force create bizhawk config file if it doesn't exist
			//if (!File.Exists(CorruptCore.bizhawkDir + Path.DirectorySeparatorChar + "config.ini"))
			//Hooks.BIZHAWK_MAINFORM_SAVECONFIG();

			//If it's attached, lie to vanguard
			if (VanguardCore.attached)
				VanguardConnector.ImplyClientConnected();
		}

	}
}
