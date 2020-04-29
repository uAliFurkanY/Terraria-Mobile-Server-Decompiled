using Steamworks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria.Social.Base;

namespace Terraria.Social.Steam
{
	public class CloudSocialModule : Terraria.Social.Base.CloudSocialModule
	{
		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Shutdown()
		{
		}

		public override List<string> GetFiles(string matchPattern)
		{
			matchPattern = "^" + matchPattern + "$";
			List<string> list = new List<string>();
			int fileCount = SteamRemoteStorage.GetFileCount();
			Regex regex = new Regex(matchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			for (int i = 0; i < fileCount; i++)
			{
				int pnFileSizeInBytes;
				string fileNameAndSize = SteamRemoteStorage.GetFileNameAndSize(i, out pnFileSizeInBytes);
				Match match = regex.Match(fileNameAndSize);
				if (match.Length > 0)
				{
					list.Add(fileNameAndSize);
				}
			}
			return list;
		}

		public override bool Write(string path, byte[] data, int length)
		{
			return SteamRemoteStorage.FileWrite(path, data, length);
		}

		public override int GetFileSize(string path)
		{
			return SteamRemoteStorage.GetFileSize(path);
		}

		public override void Read(string path, byte[] buffer, int size)
		{
			SteamRemoteStorage.FileRead(path, buffer, size);
		}

		public override bool HasFile(string path)
		{
			return SteamRemoteStorage.FileExists(path);
		}

		public override bool Delete(string path)
		{
			return SteamRemoteStorage.FileDelete(path);
		}
	}
}
