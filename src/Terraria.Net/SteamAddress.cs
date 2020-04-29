using Steamworks;

namespace Terraria.Net
{
	public class SteamAddress : RemoteAddress
	{
		public readonly CSteamID SteamId;

		private string _friendlyName;

		public SteamAddress(CSteamID steamId)
		{
			Type = AddressType.Steam;
			SteamId = steamId;
		}

		public override string ToString()
		{
			string str = (SteamId.m_SteamID % 2uL).ToString();
			string str2 = ((SteamId.m_SteamID - (76561197960265728L + SteamId.m_SteamID % 2uL)) / 2uL).ToString();
			return "STEAM_0:" + str + ":" + str2;
		}

		public override string GetIdentifier()
		{
			return ToString();
		}

		public override bool IsLocalHost()
		{
			if (Program.LaunchParameters.ContainsKey("-localsteamid"))
			{
				return Program.LaunchParameters["-localsteamid"].Equals(SteamId.m_SteamID.ToString());
			}
			return false;
		}

		public override string GetFriendlyName()
		{
			if (_friendlyName == null)
			{
				_friendlyName = SteamFriends.GetFriendPersonaName(SteamId);
			}
			return _friendlyName;
		}
	}
}
