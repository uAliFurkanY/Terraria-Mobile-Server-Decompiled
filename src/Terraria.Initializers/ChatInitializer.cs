using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace Terraria.Initializers
{
	internal static class ChatInitializer
	{
		public static void Load()
		{
			ChatManager.Register<ColorTagHandler>(new string[2]
			{
				"c",
				"color"
			});
			ChatManager.Register<ItemTagHandler>(new string[2]
			{
				"i",
				"item"
			});
			ChatManager.Register<NameTagHandler>(new string[2]
			{
				"n",
				"name"
			});
			ChatManager.Register<AchievementTagHandler>(new string[2]
			{
				"a",
				"achievement"
			});
		}
	}
}
