using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.UI.Chat;

namespace Terraria.GameContent.Events
{
	internal class BlameNPCTest
	{
		public static Dictionary<int, int> npcTypes = new Dictionary<int, int>();

		public static List<KeyValuePair<int, int>> mostSeen = new List<KeyValuePair<int, int>>();

		public static void Update(int newEntry)
		{
			if (npcTypes.ContainsKey(newEntry))
			{
				npcTypes[newEntry]++;
			}
			else
			{
				npcTypes[newEntry] = 1;
			}
			mostSeen = npcTypes.ToList();
			mostSeen.Sort((KeyValuePair<int, int> x, KeyValuePair<int, int> y) => x.Value.CompareTo(y.Value));
		}

		public static void Draw(SpriteBatch sb)
		{
			if (!Main.netDiag && !Main.showFrameRate)
			{
				for (int i = 0; i < mostSeen.Count; i++)
				{
					int num = 200 + i % 13 * 100;
					int num2 = 200 + i / 13 * 30;
					ChatManager.DrawColorCodedString(sb, Main.fontItemStack, mostSeen[i].Key + " (" + mostSeen[i].Value + ")", new Vector2(num, num2), Color.White, 0f, Vector2.Zero, Vector2.One);
				}
			}
		}
	}
}
