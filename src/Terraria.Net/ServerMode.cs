using System;

namespace Terraria.Net
{
	[Flags]
	public enum ServerMode : byte
	{
		None = 0x0,
		Lobby = 0x1,
		FriendsCanJoin = 0x2,
		FriendsOfFriends = 0x4
	}
}
