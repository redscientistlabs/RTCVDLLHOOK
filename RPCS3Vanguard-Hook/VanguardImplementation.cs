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

namespace RPCS3Vanguard_Hook
{
	public class MemoryDomainElfDomain : IMemoryDomain
	{
		public string Name => "00000000-1FFF0000 : ELFs";
		public bool BigEndian => true;
		public long Size => 0x1FFF0000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainElfDomain()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0x00010000, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0x00010000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainUserMemory : IMemoryDomain
	{
		public string Name => "20000000-30000000";
		public bool BigEndian => true;
		public long Size => 0x10000000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainUserMemory()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0x20000000, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0x20000000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainSPUMemory : IMemoryDomain
	{
		public string Name => "E0000000-100000000 : SPU Reserved";
		public bool BigEndian => true;
		public long Size => 0x20000000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainSPUMemory()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0xE0000000, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0xE0000000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainOVLMemory : IMemoryDomain
	{
		public string Name => "30000000-40000000 : Overlay";
		public bool BigEndian => true;
		public long Size => 0x10000000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainOVLMemory()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0x30000000, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0x30000000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainMMapper : IMemoryDomain
	{
		public string Name => "B0000000-C0000000";
		public bool BigEndian => true;
		public long Size => 0x10000000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainMMapper()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0xB0000000, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0xB0000000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainHeap : IMemoryDomain
	{
		public string Name => "50000000-B0000000";
		public bool BigEndian => true;
		public long Size => 0x60000000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainHeap()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0x50000000, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0x50000000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainWholeVM : IMemoryDomain
	{
		public string Name => "Entire Virtual Memory";
		public bool BigEndian => true;
		public long Size => 0x100000000;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainWholeVM()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address, (char)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr, buffer);
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
		[DllImport("RPCS3.exe")]
		public static extern char ManagedWrapper_peekbyte(long addr);
		[DllImport("RPCS3.exe")]
		public static extern void ManagedWrapper_pokebyte(long addr, char val);
		[DllImport("RPCS3.exe")]
		public static extern void ManagedWrapper_savesavestate([MarshalAs(UnmanagedType.LPStr)] string filename);
		[DllImport("RPCS3.exe")]
		public static extern void ManagedWrapper_loadsavestate([MarshalAs(UnmanagedType.LPStr)] string filename);
		[DllImport("RPCS3.exe")]
		public static extern void ManagedWrapper_pause();
		[DllImport("RPCS3.exe")]
		public static extern void ManagedWrapper_resume();

		public static uint VM_READB(long addr, uint buf)
        {
			return ManagedWrapper_peekbyte(addr);
		}
		public static void VM_WRITEB(long addr, char buf)
		{
			ManagedWrapper_pokebyte(addr, buf);
		}
		public static void SaveVMState(string path)
        {
			ManagedWrapper_savesavestate(path);
			//File.Create(path); //create dummy file for now, savestate manager isn't working for now
		}
		public static void LoadVMState(string filename)
		{
			ManagedWrapper_loadsavestate(filename);
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
			StepActions.ClearStepBlastUnits();
			RtcClock.ResetCount();
			LoadVMState(path);
			return true;
		}
		
		public static string GetGameName()
		{
			string gamename = "IGNORE";
			
			return gamename;
		}
		public static string GetShortGameName()
        {
			string gamename = GetGameName();
			string shortgamename = gamename.Trim().Replace(" ", "").Substring(0, 8);
			return shortgamename;
        }
		public static void LoadROM(string rompath)
		{
			//vanguard_setDVDPath(dvd);
		}
		public static string GetROM()
        {
			return "";
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
			if (VanguardImplementation.connector.netcoreStatus != RTCV.NetCore.Enums.NetworkStatus.CONNECTED)
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
			MemoryDomainElfDomain elfdomn = new MemoryDomainElfDomain();
			interfaces.Add(new MemoryDomainProxy(elfdomn));
			MemoryDomainUserMemory userMemory = new MemoryDomainUserMemory();
			interfaces.Add(new MemoryDomainProxy(userMemory));
			MemoryDomainOVLMemory oVLMemory = new MemoryDomainOVLMemory();
			interfaces.Add(new MemoryDomainProxy(oVLMemory));
			MemoryDomainHeap heap = new MemoryDomainHeap();
			interfaces.Add(new MemoryDomainProxy(heap));
			MemoryDomainMMapper mmapper = new MemoryDomainMMapper();
			interfaces.Add(new MemoryDomainProxy(mmapper));
			MemoryDomainSPUMemory sPUMemory = new MemoryDomainSPUMemory();
			interfaces.Add(new MemoryDomainProxy(sPUMemory));
			MemoryDomainWholeVM entireVM = new MemoryDomainWholeVM();
			interfaces.Add(new MemoryDomainProxy(entireVM));
			return interfaces.ToArray();
			
        }
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
						//VanguardCore.LOAD_GAME_START(fileName);
						
					}
					break;
				case RTCV.NetCore.Commands.Remote.PreCorruptAction:
					SyncObjectSingleton.EmuThreadExecute(ManagedWrapper_pause, true);
					break;

				case RTCV.NetCore.Commands.Remote.PostCorruptAction:
					SyncObjectSingleton.EmuThreadExecute(ManagedWrapper_resume, true);
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
