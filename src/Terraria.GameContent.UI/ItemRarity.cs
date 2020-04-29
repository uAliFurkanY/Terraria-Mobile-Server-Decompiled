using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.GameContent.UI
{
	public class ItemRarity
	{
		private static Dictionary<int, Color> rarities = new Dictionary<int, Color>();

		public static void Initialize()
		{
			rarities.Clear();
			rarities.Add(-11, Colors.RarityAmber);
			rarities.Add(-1, Colors.RarityTrash);
			rarities.Add(1, Colors.RarityBlue);
			rarities.Add(2, Colors.RarityGreen);
			rarities.Add(3, Colors.RarityOrange);
			rarities.Add(4, Colors.RarityRed);
			rarities.Add(5, Colors.RarityPink);
			rarities.Add(6, Colors.RarityPurple);
			rarities.Add(7, Colors.RarityLime);
			rarities.Add(8, Colors.RarityYellow);
			rarities.Add(9, Colors.RarityCyan);
		}

		public static Color GetColor(int rarity)
		{
			Color result = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
			if (rarities.ContainsKey(rarity))
			{
				return rarities[rarity];
			}
			return result;
		}
	}
}
