using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RTCV.Common;
using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.Vanguard;
using RTCV.UI;
using RTCV.NetCore.Commands;
using System.IO;
using System.IO.Compression;

namespace RetroarchVanguard_Hook
{
    public class DummyMemoryDomain : IMemoryDomain
    {
        public string Name => "DUMMY";

        public long Size => 0;

        public int WordSize => 1;

        public bool BigEndian => false;

        public byte PeekByte(long addr)
        {
			return 0;
        }

        public byte[] PeekBytes(long address, int length)
        {
            return new byte[length];
        }

        public void PokeByte(long addr, byte val)
        {
            
        }
	}

	public class RAMemoryDomain : IMemoryDomain
	{
		public uint Id;
		public string Name { get; }
		public bool BigEndian { get; }
		public long Size { get; }
		public int WordSize { get; }
		public override string ToString() => Name;
		public RAMemoryDomain(uint id)
		{
			Id = id;
			Name = VanguardImplementation.GetMemoryRegionName(id, false);
			BigEndian = VanguardImplementation.VanguardWrapper_ismemregionbigendian(id);
			Size = (long)VanguardImplementation.VanguardWrapper_getmemsize(id);
			WordSize = VanguardImplementation.GetWordSize();
		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VanguardWrapper_pokebyte(Id, address, val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			return VanguardImplementation.VanguardWrapper_peekbyte(Id, addr);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}

	public class FallbackRAMemoryDomain : IMemoryDomain
	{
		public string Name { get; }
		public bool BigEndian { get; }
		public long Size { get; }
		public int WordSize { get; }
		public override string ToString() => Name;
		public FallbackRAMemoryDomain()
		{
			Name = VanguardImplementation.GetMemoryRegionName(0, true); // SYSTEM_RAM
			BigEndian = VanguardImplementation.IsCoreBigEndian(); // WILL be wrong for some cores that aren't taken into account by this function
			Size = (long)VanguardImplementation.VanguardWrapper_getmemsize_alt(2);
			WordSize = VanguardImplementation.GetWordSize();
		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VanguardWrapper_pokebyte_alt(2, address, val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			return VanguardImplementation.VanguardWrapper_peekbyte_alt(2, addr);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class VanguardImplementation
	{
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern byte VanguardWrapper_peekbyte(uint id, long addr);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void VanguardWrapper_pokebyte(uint id, long addr, byte val);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern byte VanguardWrapper_peekbyte_alt(uint type, long addr);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void VanguardWrapper_pokebyte_alt(uint type, long addr, byte val);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ulong VanguardWrapper_getmemsize(uint id);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ulong VanguardWrapper_getmemsize_alt(uint type);

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr VanguardWrapper_getmemname(uint id);

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr VanguardWrapper_getmemname_alt(uint type);

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern uint VanguardWrapper_getmemdesccount();
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void VanguardWrapper_savestate([MarshalAs(UnmanagedType.LPStr)] string path);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void VanguardWrapper_loadstate([MarshalAs(UnmanagedType.LPStr)] string path);

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr VanguardWrapper_getrompath();

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr VanguardWrapper_getcorepath();

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr VanguardWrapper_getcorename();

		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void VanguardWrapper_loadcontent([MarshalAs(UnmanagedType.LPStr)] string core, [MarshalAs(UnmanagedType.LPStr)] string rompath);
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		//[return: MarshalAs(UnmanagedType.LPStr)]
		public static extern IntPtr VanguardWrapper_getcontentname();
		[DllImport("RetroArch-msvc2019.exe", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool VanguardWrapper_ismemregionbigendian(uint id);

		public static void SaveVMState(string path)
        {
			VanguardWrapper_savestate(path);
			Thread.Sleep(1000);
			//File.Create(path); //create dummy file for now, savestate manager isn't working for now
		}
		public static void LoadVMState(string filename)
		{
			VanguardWrapper_loadstate(filename);
			Thread.Sleep(1000);
		}
		public static string GetStateName()
        {
			return "";
        }
		public static string SaveSavestate(string Key, bool threadSave = false)
		{
			string quickSlotName = Key + ".timejump";
			string prefix = VanguardCore.GameName;
			string path = Path.Combine(RtcCore.workingDir, "SESSION", prefix + "." + quickSlotName + ".State");
			VanguardImplementation.SaveVMState(path);
			return path;
		}

		public static bool LoadSavestate(string path, StashKeySavestateLocation stateLocation = StashKeySavestateLocation.DEFAULTVALUE)
		{
			LoadVMState(path);
			return true;
		}
		
		public static string GetGameName()
		{
			var ret = VanguardWrapper_getcontentname();
			
			return Marshal.PtrToStringAnsi(ret).ToString().Trim();
		}
		public static string GetShortGameName()
        {
			string gamename = GetGameName();
			string shortgamename = gamename.Trim().Replace(" ", "").Substring(0, 8);
			return shortgamename;
        }
		public static void LoadROM(string core, string rompath)
		{
			VanguardWrapper_loadcontent(core, rompath);
		}
		public static string GetROM()
		{
			var ret = VanguardWrapper_getrompath();

			return Marshal.PtrToStringAnsi(ret).ToString().Trim();
		}
		public static string GetCoreName()
		{
			var ret = VanguardWrapper_getcorename();

			return Marshal.PtrToStringAnsi(ret).ToString().Trim();
		}
		public static string GetCorePath()
		{
			var ret = VanguardWrapper_getcorepath();

			return Marshal.PtrToStringAnsi(ret).ToString().Trim();
		}
		public static string GetMemoryRegionName(uint id, bool fallback)
		{
			if (!fallback)
            {
				var ret = VanguardWrapper_getmemname(id);

				return Marshal.PtrToStringAnsi(ret).ToString().Trim();
			}
			else
            {
				var ret = VanguardWrapper_getmemname_alt(2 /* SYSTEM_RAM */);

				return Marshal.PtrToStringAnsi(ret).ToString().Trim();
			}
		}
		// used for cores that don't use memory descriptors
		public static bool IsCoreBigEndian()
        {
			var core = VanguardCore.SystemCore;
			if (core == null)
            {
				return false;
            }
			// set big endian if core is for a known big endian system
            switch (core)
			{
				// todo: set big endian for each (ugh) core that is big endian and is known to use get_memory_data() instead of memory descriptors
				case "\\dolphin_libretro.dll":
					return true;
				case "\\parallel_n64_libretro.dll":
					return true;
				default: break;
            }
            return false;
        }
		public static int GetWordSize()
		{
			var core = VanguardCore.SystemCore;
			if (core == null)
            {
				return 4; // fallback
            }
            // set word size different from 4 if core is for a known system with a word size different from that
            switch (core)
            {
				// todo: set word size for each (ugh) core that is known to use get_memory_data() instead of memory descriptors
				default: break;
            }
            return 4; // fallback
		}
		//[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
		//delegate
		public static RTCV.Vanguard.VanguardConnector connector = null;
        public static void StartClient()
        {
            try
            {
                ConsoleEx.WriteLine("Starting Vanguard Client");
                Thread.Sleep(500);
                var spec = new NetCoreReceiver();
                spec.Attached = VanguardCore.attached;
                spec.MessageReceived += OnMessageRecieved;
                connector = new RTCV.Vanguard.VanguardConnector(spec);
				//while(true)
    //            {
    //            }
            }
            catch (Exception ex)
            {
                ConsoleEx.WriteLine("ERROR");
                throw new RTCV.NetCore.AbortEverythingException();
            }
        }
        public static void RestartClient()
        {
            connector?.Kill();
            connector = null;
            StartClient();
        }
		public static void RefreshDomains()
        {
			if (VanguardImplementation.connector.netcoreStatus != RTCV.NetCore.Enums.NetworkStatus.CONNECTED || (VanguardWrapper_getmemdesccount() == 0 && VanguardImplementation.VanguardWrapper_getmemsize_alt(2) == 0))
                return;
			PartialSpec gameDone = new PartialSpec("VanguardSpec");
			gameDone[VSPEC.MEMORYDOMAINS_INTERFACES] = GetInterfaces();
			//gameDone[VSPEC.GAMENAME] = GetGameName();
			AllSpec.VanguardSpec.Update(gameDone);
			LocalNetCoreRouter.Route(RTCV.NetCore.Endpoints.CorruptCore, RTCV.NetCore.Commands.Remote.EventDomainsUpdated, true, true);
		}
		public static MemoryDomainProxy[] GetInterfaces()
        {
			Console.WriteLine($"getInterfaces()");
			List<MemoryDomainProxy> interfaces = new List<MemoryDomainProxy>();
			for (uint i = 0; i < VanguardWrapper_getmemdesccount(); i++)
            {
				interfaces.Add(new MemoryDomainProxy(new RAMemoryDomain(i)));
            }
			if (VanguardWrapper_getmemdesccount() == 0)
            {
				interfaces.Add(new MemoryDomainProxy(new FallbackRAMemoryDomain()));
            }
			return interfaces.Count > 0 ? interfaces.ToArray() : new MemoryDomainProxy[] {new MemoryDomainProxy(new DummyMemoryDomain()) };
			
        }
		public static string CoreToLoad;
        private static void OnMessageRecieved(object sender, NetCoreEventArgs e)
        {
            var message = e.message;
            var simpleMessage = message as NetCoreSimpleMessage;
            var advancedMessage = message as NetCoreAdvancedMessage;

            ConsoleEx.WriteLine(message.Type);
			switch (message.Type) //Handle received messages here
			{
				case RTCV.NetCore.Commands.Remote.AllSpecSent:
					{
						if (VanguardCore.FirstConnect)
						{
							SyncObjectSingleton.FormExecute(() => { ; });
							
							VanguardCore.FirstConnect = false;
						}
						//VanguardCore.LOAD_GAME_DONE();
						RefreshDomains();
					}
					break;
				case RTCV.NetCore.Commands.Basic.SaveSavestate:
					SyncObjectSingleton.EmuThreadExecute(() => { e.setReturnValue(SaveSavestate(advancedMessage.objectValue as string)); }, true);
					break;

				case RTCV.NetCore.Commands.Basic.LoadSavestate:
					{
						var cmd = advancedMessage.objectValue as object[];
						var path = cmd[0] as string;
						var location = (StashKeySavestateLocation)cmd[1];
						SyncObjectSingleton.EmuThreadExecute(() => { e.setReturnValue(LoadSavestate(path, location)); }, true);
						break;
					}

				case RTCV.NetCore.Commands.Remote.LoadROM:
					{
						var fileName = advancedMessage.objectValue as string;
						if (VanguardCore.OpenRomFilename != fileName)
                        {
							VanguardCore.LOAD_GAME_START(CoreToLoad, fileName);
						}
					}
					break;
				case RTCV.NetCore.Commands.Remote.PreCorruptAction:
					break;

				case RTCV.NetCore.Commands.Remote.PostCorruptAction:
                    {
					}
					break;
				case RTCV.NetCore.Commands.Remote.CloseGame:
					{
						//SyncObjectSingleton.FormExecute(() => { Hooks.CLOSE_GAME(true); });
					}
					break;

				case RTCV.NetCore.Commands.Remote.DomainGetDomains:
					SyncObjectSingleton.FormExecute(() =>
					{
						e.setReturnValue(GetInterfaces());
					});
					break;

				case RTCV.NetCore.Commands.Remote.DomainRefreshDomains:
					RefreshDomains();
					break;

				case RTCV.NetCore.Commands.Remote.KeySetSyncSettings:
					//SyncObjectSingleton.FormExecute(() => { Hooks.BIZHAWK_GETSET_SYNCSETTINGS = (string)advancedMessage.objectValue; });
					CoreToLoad = (string)advancedMessage.objectValue; ;
					break;

				case RTCV.NetCore.Commands.Emulator.GetRealtimeAPI:
					e.setReturnValue(VanguardCore.RTE_API);
					break;


				case RTCV.NetCore.Commands.Remote.EventEmuStarted:
					//if (RTC_StockpileManager.BackupedState == null)
					//S.GET<RTC_Core_Form>().AutoCorrupt = false;


					//Todo
					//RTC_NetcoreImplementation.SendCommandToBizhawk(new RTC_Command("REMOTE_PUSHVMDS) { objectValue = MemoryDomains.VmdPool.Values.Select(it => (it as VirtualMemoryDomain).Proto).ToArray() }, true, true);

					//Thread.Sleep(100);

					//		if (RTC_StockpileManager.BackupedState != null)
					//			S.GET<RTC_MemoryDomains_Form>().RefreshDomainsAndKeepSelected(RTC_StockpileManager.BackupedState.SelectedDomains.ToArray());

					//		if (S.GET<RTC_Core_Form>().cbUseGameProtection.Checked)
					//			RTC_GameProtection.Start();

					break;
				case RTCV.NetCore.Commands.Remote.OpenHexEditor:
					SyncObjectSingleton.FormExecute(() => LocalNetCoreRouter.Route("HEXEDITOR", Remote.OpenHexEditor, true));
					break;
				case RTCV.NetCore.Commands.Remote.EventCloseEmulator:
					Environment.Exit(-1);
					break;
			}
		}
    }
}
