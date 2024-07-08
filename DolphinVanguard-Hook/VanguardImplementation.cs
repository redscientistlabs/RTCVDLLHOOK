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
using System.Windows;

namespace DolphinVanguard_Hook
{
	public class MemoryDomainSRAMDomain : IMemoryDomain
	{
		public string Name => "SRAM";
		public bool BigEndian => true;
		public long Size => 0x017FFFFF;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainSRAMDomain()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0x80000000, (byte)val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0x80000000, buffer);
		}
		public byte[] PeekBytes(long address, int length)
		{

			var returnArray = new byte[length];
			for (var i = 0; i < length; i++)
				returnArray[i] = PeekByte(address + i);
			return returnArray;
		}
	}
	public class MemoryDomainEXRAMDomain : IMemoryDomain
	{
		public string Name => "EXRAM";
		public bool BigEndian => true;
		public long Size => 0x03FFFFFF;
		public int WordSize => 4;
		public override string ToString() => Name;
		public MemoryDomainEXRAMDomain()
		{

		}
		public void PokeByte(long address, byte val)
		{
			if (address > Size)
			{
				return;
			}
			VanguardImplementation.VM_WRITEB(address + 0x90000000, val);
		}
		public byte PeekByte(long addr)
		{
			if (addr > Size)
			{
				return 0;
			}
			uint buffer = 0;
			return (byte)VanguardImplementation.VM_READB(addr + 0x90000000, buffer);
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
		public static bool enableRTC = true;

		[DllImport("Dolphin.exe")]
		public static extern byte ManagedWrapper_peekbyte(long addr);
		[DllImport("Dolphin.exe")]
		public static extern void ManagedWrapper_pokebyte(long addr, byte val);
		[DllImport("Dolphin.exe")]
		public static extern bool ManagedWrapper_savesavestate(string filename, bool wait);
		[DllImport("Dolphin.exe")]
		public static extern bool ManagedWrapper_loadsavestate(string filename);
		[DllImport("Dolphin.exe")]
		public static extern void ManagedWrapper_pause();
		[DllImport("Dolphin.exe")]
		public static extern void ManagedWrapper_resume();

		public static void ReloadState()
        {
			var path = Path.Combine(RtcCore.workingDir, "SESSION", "Dolphintmp.savestat");
			SyncObjectSingleton.EmuThreadExecute(() =>
			{
				ManagedWrapper_savesavestate(path, false);
				ManagedWrapper_loadsavestate(path);
			}, true);
		}

		public static byte VM_READB(long addr, uint buf)
        {
			return ManagedWrapper_peekbyte(addr);
		}
		public static void VM_WRITEB(long addr, byte buf)
		{
			ManagedWrapper_pokebyte(addr, buf);
		}
		public static void SaveVMState(string path)
        {
			ManagedWrapper_savesavestate(path, false);
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
		public static void StopClient()
		{
			connector?.Kill();
			connector = null;
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
			MemoryDomainSRAMDomain SRAMdomn = new MemoryDomainSRAMDomain();
			interfaces.Add(new MemoryDomainProxy(SRAMdomn));
            MemoryDomainEXRAMDomain EXRAMdomn = new MemoryDomainEXRAMDomain();
            interfaces.Add(new MemoryDomainProxy(EXRAMdomn));
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
					{
						SyncObjectSingleton.EmuThreadExecute(() => { e.setReturnValue(SaveSavestate(advancedMessage.objectValue as string)); }, true);
						break;
					}
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
                    {
						//var path = Path.Combine(RtcCore.workingDir, "Dolphintmp.savestat");
						SyncObjectSingleton.EmuThreadExecute(() =>
						{
							ManagedWrapper_resume();
							//ManagedWrapper_savesavestate(path);
							//ManagedWrapper_loadsavestate(path);
						}, true);
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
