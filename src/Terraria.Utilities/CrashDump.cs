using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Terraria.Utilities
{
	public class CrashDump
	{
		internal enum MINIDUMP_TYPE
		{
			MiniDumpNormal = 0,
			MiniDumpWithDataSegs = 1,
			MiniDumpWithFullMemory = 2,
			MiniDumpWithHandleData = 4,
			MiniDumpFilterMemory = 8,
			MiniDumpScanMemory = 0x10,
			MiniDumpWithUnloadedModules = 0x20,
			MiniDumpWithIndirectlyReferencedMemory = 0x40,
			MiniDumpFilterModulePaths = 0x80,
			MiniDumpWithProcessThreadData = 0x100,
			MiniDumpWithPrivateReadWriteMemory = 0x200,
			MiniDumpWithoutOptionalData = 0x400,
			MiniDumpWithFullMemoryInfo = 0x800,
			MiniDumpWithThreadInfo = 0x1000,
			MiniDumpWithCodeSegs = 0x2000
		}

		[DllImport("dbghelp.dll")]
		private static extern bool MiniDumpWriteDump(IntPtr hProcess, int ProcessId, IntPtr hFile, MINIDUMP_TYPE DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallackParam);

		public static void Create()
		{
			DateTime dateTime = DateTime.Now.ToLocalTime();
			string path = "TerrariaServer" + Main.versionNumber + " " + dateTime.Year.ToString("D4") + "-" + dateTime.Month.ToString("D2") + "-" + dateTime.Day.ToString("D2") + " " + dateTime.Hour.ToString("D2") + "_" + dateTime.Minute.ToString("D2") + "_" + dateTime.Second.ToString("D2") + ".dmp";
			Create(path);
		}

		public static void CreateFull()
		{
			DateTime dateTime = DateTime.Now.ToLocalTime();
			string path = "DMP-FULL TerrariaServer" + Main.versionNumber + " " + dateTime.Year.ToString("D4") + "-" + dateTime.Month.ToString("D2") + "-" + dateTime.Day.ToString("D2") + " " + dateTime.Hour.ToString("D2") + "_" + dateTime.Minute.ToString("D2") + "_" + dateTime.Second.ToString("D2") + ".dmp";
			using (FileStream fileStream = File.Create(path))
			{
				Process currentProcess = Process.GetCurrentProcess();
				MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fileStream.SafeFileHandle.DangerousGetHandle(), MINIDUMP_TYPE.MiniDumpWithFullMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
		}

		public static void Create(string path)
		{
			bool flag = Program.LaunchParameters.ContainsKey("-fulldump");
			using (FileStream fileStream = File.Create(path))
			{
				Process currentProcess = Process.GetCurrentProcess();
				MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fileStream.SafeFileHandle.DangerousGetHandle(), flag ? MINIDUMP_TYPE.MiniDumpWithFullMemory : MINIDUMP_TYPE.MiniDumpWithIndirectlyReferencedMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}
}
