using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Achievements;

namespace Terraria.UI
{
	public class AchievementsUI
	{
		public static void Open()
		{
			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";
			Main.achievementsWindow = true;
			Main.InGameUI.SetState(Main.AchievementsMenu);
		}

		public static void OpenAndGoto(Achievement achievement)
		{
			Open();
			Main.AchievementsMenu.GotoAchievement(achievement);
		}

		public static void Close()
		{
			Main.achievementsWindow = false;
			Main.PlaySound(11);
			if (!Main.gameMenu)
			{
				Main.playerInventory = true;
			}
			Main.InGameUI.SetState(null);
		}

		public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (!Main.gameMenu && Main.player[Main.myPlayer].dead && !Main.player[Main.myPlayer].ghost)
			{
				Close();
				Main.playerInventory = false;
			}
			else if (!Main.gameMenu)
			{
				Main.mouseText = false;
				Main.instance.GUIBarsDraw();
				if (!Main.achievementsWindow)
				{
					Main.InGameUI.SetState(null);
				}
				Main.instance.DrawMouseOver();
				Vector2 value = Main.DrawThickCursor();
				spriteBatch.Draw(Main.cursorTextures[0], new Vector2(Main.mouseX, Main.mouseY) + value + Vector2.One, null, new Color((int)((float)(int)Main.cursorColor.R * 0.2f), (int)((float)(int)Main.cursorColor.G * 0.2f), (int)((float)(int)Main.cursorColor.B * 0.2f), (int)((float)(int)Main.cursorColor.A * 0.5f)), 0f, default(Vector2), Main.cursorScale * 1.1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(Main.cursorTextures[0], new Vector2(Main.mouseX, Main.mouseY) + value, null, Main.cursorColor, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
			}
		}

		public static void MouseOver()
		{
			if (Main.achievementsWindow && Main.InGameUI.IsElementUnderMouse())
			{
				Main.mouseText = true;
			}
		}
	}
}
