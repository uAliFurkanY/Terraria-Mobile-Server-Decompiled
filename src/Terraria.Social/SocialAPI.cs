using System.Collections.Generic;
using Terraria.Social.Base;
using Terraria.Social.Steam;

namespace Terraria.Social
{
	public static class SocialAPI
	{
		private static SocialMode _mode = SocialMode.None;

		public static Terraria.Social.Base.FriendsSocialModule Friends;

		public static Terraria.Social.Base.AchievementsSocialModule Achievements;

		public static Terraria.Social.Base.CloudSocialModule Cloud;

		public static Terraria.Social.Base.NetSocialModule Network;

		private static List<ISocialModule> _modules;

		public static SocialMode Mode => _mode;

		public static void Initialize(SocialMode? mode = null)
		{
			if (!mode.HasValue)
			{
				mode = SocialMode.None;
			}
			_mode = mode.Value;
			_modules = new List<ISocialModule>();
			SocialMode mode2 = Mode;
			if (mode2 == SocialMode.Steam)
			{
				LoadSteam();
			}
			foreach (ISocialModule module in _modules)
			{
				module.Initialize();
			}
		}

		public static void Shutdown()
		{
			_modules.Reverse();
			foreach (ISocialModule module in _modules)
			{
				module.Shutdown();
			}
		}

		private static T LoadModule<T>() where T : ISocialModule, new()
		{
			T val = new T();
			_modules.Add(val);
			return val;
		}

		private static T LoadModule<T>(T module) where T : ISocialModule
		{
			_modules.Add(module);
			return module;
		}

		private static void LoadSteam()
		{
			LoadModule<CoreSocialModule>();
			Friends = LoadModule<Terraria.Social.Steam.FriendsSocialModule>();
			Achievements = LoadModule<Terraria.Social.Steam.AchievementsSocialModule>();
			Cloud = LoadModule<Terraria.Social.Steam.CloudSocialModule>();
			Network = LoadModule<NetServerSocialModule>();
		}
	}
}
