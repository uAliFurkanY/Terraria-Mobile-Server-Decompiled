using Steamworks;
using System;
using System.Threading;
using Terraria.Social.Base;

namespace Terraria.Social.Steam
{
	internal class AchievementsSocialModule : Terraria.Social.Base.AchievementsSocialModule
	{
		private const string FILE_NAME = "/achievements-steam.dat";

		private Callback<UserStatsReceived_t> _userStatsReceived;

		private bool _areStatsReceived;

		public override void Initialize()
		{
			_userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			SteamUserStats.RequestCurrentStats();
			while (!_areStatsReceived)
			{
				CoreSocialModule.Pulse();
				Thread.Sleep(10);
			}
		}

		public override void Shutdown()
		{
			StoreStats();
		}

		public override bool IsAchievementCompleted(string name)
		{
			bool pbAchieved;
			if (SteamUserStats.GetAchievement(name, out pbAchieved))
			{
				return pbAchieved;
			}
			return false;
		}

		public override byte[] GetEncryptionKey()
		{
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(SteamUser.GetSteamID().m_SteamID);
			Array.Copy(bytes, array, 8);
			Array.Copy(bytes, 0, array, 8, 8);
			return array;
		}

		public override string GetSavePath()
		{
			return "/achievements-steam.dat";
		}

		public override void UpdateIntStat(string name, int value)
		{
			int pData;
			SteamUserStats.GetStat(name, out pData);
			if (pData < value)
			{
				SteamUserStats.SetStat(name, value);
			}
		}

		public override void UpdateFloatStat(string name, float value)
		{
			float pData;
			SteamUserStats.GetStat(name, out pData);
			if (pData < value)
			{
				SteamUserStats.SetStat(name, value);
			}
		}

		public override void StoreStats()
		{
			SteamUserStats.StoreStats();
		}

		public override void CompleteAchievement(string name)
		{
			SteamUserStats.SetAchievement(name);
		}

		private void OnUserStatsReceived(UserStatsReceived_t results)
		{
			if (results.m_nGameID == 105600 && results.m_steamIDUser == SteamUser.GetSteamID())
			{
				_areStatsReceived = true;
			}
		}
	}
}
