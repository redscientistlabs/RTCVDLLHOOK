using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenGL;
using EasyHook;
using RTCV.Common;
using RTCV.CorruptCore;
using RTCV.NetCore;
using RTCV.Vanguard;
using RTCV.UI;
using RTCV.NetCore.Commands;
using System.IO;

namespace XemuVanguardHook
{
	public class MemoryDomainCPUMemory : IMemoryDomain, ICodeCavable
	{
		public string Name => "System Memory";
		public bool BigEndian => false;
		public long Size => VanguardImplementation.vanguard_getMemorySize();
		public int WordSize => 4;

        public ICodeCavesDomain CodeCaves { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string ToString() => Name;
		public MemoryDomainCPUMemory()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.GPA_WRITEB(address, val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			byte buffer = 0;
			return (byte)VanguardImplementation.GPA_READB(addr, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}

        public byte[] GetMemory()
        {
			return PeekBytes(0, (int)Size);
        }
    }
	public class MemoryDomainNV2A : IMemoryDomain
	{
		public string Name => "NV2A Registers";
		public bool BigEndian => false;
		public long Size => 0xFFFFFF+1;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainNV2A()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			address += 0xFD000000;
			VanguardImplementation.GPA_WRITEB(address, val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			addr += 0xFD000000;
			byte buffer = 0;
			return (byte)VanguardImplementation.GPA_READB(addr, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainAPU : IMemoryDomain
	{
		public string Name => "APU Registers";
		public bool BigEndian => false;
		public long Size => 0x7FFFF;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainAPU()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			address += 0xFE800000;
			VanguardImplementation.GPA_WRITEB(address, val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			addr += 0xFE800000;
			byte buffer = 0;
			return (byte)VanguardImplementation.GPA_READB(addr, buffer);
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

        [DllImport("xemu.exe")]
        public static extern byte gpa_readb(long addr, byte buf);
		[DllImport("xemu.exe")]
		public static extern void gpa_writeb(long addr, byte buf);
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void vanguard_savevm_state(char* cmd);
        [DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void vanguard_loadvm_state(char* cmd);
        [DllImport("xemu.exe")]
        public static extern int vanguard_getMemorySize();
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static extern void vanguard_setMemorySize(int size);
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static extern string vanguard_getHDDPath();
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static extern void vanguard_setHDDPath(string path);
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static extern string vanguard_getDVDPath();
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static extern void vanguard_setDVDPath(string path);
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static unsafe extern void vanguard_setMainThreadCommand(char* command);
		[DllImport("xemu.exe", CallingConvention = CallingConvention.Cdecl)]
		public static unsafe extern void vanguard_setMainThreadCommandCharArg(char* arg);
		public static byte GPA_READB(long addr, byte buf)
        {
			return gpa_readb(addr, buf);
		}
		public static void GPA_WRITEB(long addr, byte buf)
		{
			gpa_writeb(addr, buf);
		}
		public static unsafe void SaveVMState(string filename, string realfilepath)
        {
			//vanguard_savevm_state(filename);
			string cmd = "savevm " + filename;
			vanguard_savevm_state((char*)Marshal.StringToHGlobalAnsi(cmd).ToPointer());
			Thread.Sleep(1000);
			
		}
		public static unsafe void LoadVMState(string filename)
		{
			//vanguard_loadvm_state(filename);
			string cmd = "loadvm " + filename;
			vanguard_loadvm_state((char*)Marshal.StringToHGlobalAnsi(cmd).ToPointer());
			Thread.Sleep(1000);
		}
		public static string SaveSavestate(string Key, bool threadSave = false)
		{
			string quickSlotName = Key + ".timejump";
			string prefix = VanguardCore.GameName;
			//Since I can't figure out how to save vm states outside of the hdd file,
			//I'll just save them internally for now, but that makes it impossible to share the states.
			//well, since they save the state of the hdd (I think) too they would be copyrighted files anyway
			string path = Path.Combine(RtcCore.workingDir, "SESSION", prefix + "." + quickSlotName + ".State");
			//File.Create(path); //make dummy savestate file
			File.WriteAllText(path, prefix + "." + Key); //make a file that points to the savestate in the qcow2
			VanguardImplementation.SaveVMState(prefix + "." + Key, path);
			return path;
		}

		public static bool LoadSavestate(string path, StashKeySavestateLocation stateLocation = StashKeySavestateLocation.DEFAULTVALUE)
		{
			StepActions.ClearStepBlastUnits();
			RtcClock.ResetCount();
			string Key = File.ReadAllText(path);
			VanguardImplementation.LoadVMState(Key);
			return true;
		}
		public static string SaveDrive()
        {
			string drivepath = vanguard_getHDDPath();
			System.IO.FileInfo fi = new FileInfo(drivepath);
			string key = $"HDD_{RtcCore.GetRandomKey()}.{fi.Extension}";
			string destdrivepath = Path.Combine(RtcCore.workingDir, "VAULT", key);
			System.IO.File.Copy(drivepath, destdrivepath);
			return destdrivepath;
        }
		public void LoadDrive(string packagepath)
        {
			vanguard_setHDDPath(packagepath);
		}
		public static string GetGameName()
		{
			string gamename = "IGNORE";
			int i = 0;
			MemoryDomainProxy xboxsdram = MemoryDomains.GetProxy("System Memory", 0);
			while (i < xboxsdram.Size)
			{
				if (xboxsdram.PeekByte(i) == 0x58)
				{
					if (xboxsdram.PeekByte(i + 1) == 0x42)
					{
						if (xboxsdram.PeekByte(i + 2) == 0x45)
						{
							if (xboxsdram.PeekByte(i + 3) == 0x48)
							{
								//MessageBox.Show("Found an XBE!");
								int xbestart = i;
								int certificateaddress = BitConverter.ToInt32(xboxsdram.PeekBytes(xbestart + 0x0118, xbestart + 0x0118 + 0x4, true), 0) - 0x10000;
								gamename = System.Text.Encoding.ASCII.GetString(xboxsdram.PeekBytes(xbestart + certificateaddress + 0xC, xbestart + certificateaddress + 0xC + 0x50, true)).Replace("\0", "");
							}
						}
					}
				}
				i++;
			}
			return gamename;
		}
		public static string GetShortGameName()
        {
			string gamename = GetGameName();
			string shortgamename = gamename.Trim().Replace(" ", "").Substring(0, 8);
			return shortgamename;
        }
		public static void LoadDVD(string dvd)
		{
			vanguard_setDVDPath(dvd);
		}
		public static string GetDVD()
        {
			return vanguard_getDVDPath();
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
			MemoryDomainCPUMemory cpumem = new MemoryDomainCPUMemory();
			interfaces.Add(new MemoryDomainProxy(cpumem));
			MemoryDomainNV2A geforce3reg = new MemoryDomainNV2A();
			interfaces.Add(new MemoryDomainProxy(geforce3reg));
			//MemoryDomainAPU apureg = new MemoryDomainAPU(); //for some reason, xemu's code for reading and writing to the apu
															 //REQUIRES that the value be 32-bit. There's an assert function that
															//IGNORES the fact I globally disabled all assertions and crashes
														   //whenever the value is 8-bit.
														  //So I'll just comment this out for now :(
			//interfaces.Add(new MemoryDomainProxy(apureg));
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
